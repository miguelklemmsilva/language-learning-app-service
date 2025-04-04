using Core.Interfaces;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Services;

public enum StudyType
{
    Translation,
    Listening,
    Speaking
}

public class AiService(
    IChatGptService chatGptService,
    ITranslationService translationService,
    ITokenRepository tokenRepository,
    IUserService userService,
    IUserLanguageService userLanguageService,
    IVocabularyService vocabularyService)
    : IAiService
{
    public async Task<SentencesResponse> GenerateSentencesAsync(string userId)
    {
        var userTask = userService.GetUserAsync(userId);
        var issueTokenTask = tokenRepository.GetIssueTokenAsync();

        await Task.WhenAll(userTask, issueTokenTask);

        var user = await userTask;
        var issueToken = await issueTokenTask;

        Language userLanguage = user.User.ActiveLanguage ?? throw new ArgumentNullException(nameof(user.User.ActiveLanguage));
        
        var activeLanguage = await userLanguageService.GetUserLanguageAsync(userId, userLanguage);

        var activeStudyTypes = GetActiveStudyTypes(activeLanguage);

        if (activeStudyTypes.Count == 0)
            throw new Exception("User has no active exercises");

        var wordsToFetch = (int)Math.Ceiling(3f / activeStudyTypes.Count);
        var wordsToStudy = await vocabularyService.GetWordsToStudyAsync(userId, userLanguage, wordsToFetch);

        var sentenceTasks = wordsToStudy.Select(word => GenerateSentenceAsync(word, activeLanguage)).ToList();

        var sentences = await Task.WhenAll(sentenceTasks);

        return new SentencesResponse
        {
            Sentences = sentences,
            IssueToken = issueToken
        };
    }

    private static List<StudyType> GetActiveStudyTypes(UserLanguage activeLanguage)
    {
        var activeTypes = new List<StudyType>();
        if (activeLanguage.Translation) activeTypes.Add(StudyType.Translation);
        if (activeLanguage.Listening) activeTypes.Add(StudyType.Listening);
        if (activeLanguage.Speaking) activeTypes.Add(StudyType.Speaking);
        return activeTypes;
    }

    private async Task<Sentence> GenerateSentenceAsync(Word word, UserLanguage activeLanguage)
    {
        var sentenceTask =
            chatGptService.GenerateSentenceAsync(word.Word, activeLanguage.Language, activeLanguage.Country);
        var translationTask = translationService.TranslateSentenceAsync(await sentenceTask, activeLanguage.Language);

        var translatedText = await translationTask;
        var translation = translatedText?.Translations.FirstOrDefault();

        return new Sentence
        {
            Original = await sentenceTask,
            Translation = translation?.Text,
            Alignment = translation?.Alignment.Projections,
            Word = word.Word,
            Language = activeLanguage.Language,
            Country = activeLanguage.Country
        };
    }
}