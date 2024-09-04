using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AWS.Services;
using Core.Interfaces;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;
using Microsoft.CognitiveServices.Speech;

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
    ITokenService tokenService,
    IUserService userService,
    IUserLanguageService userLanguageService,
    IVocabularyService vocabularyService)
    : IAiService
{
    public async Task<SentencesResponse> GenerateSentencesAsync(string userId)
    {
        var user = await userService.GetUserAsync(userId);

        if (user.ActiveLanguage == null)
            throw new Exception("User has no active language");

        var activeLanguage = await userLanguageService.GetUserLanguageAsync(userId, user.ActiveLanguage);

        var activeStudyTypes = GetActiveStudyTypes(activeLanguage);

        if (activeStudyTypes.Count == 0)
            throw new Exception("User has no active exercises");

        var wordsToFetch = (int)Math.Ceiling(3f / activeStudyTypes.Count);
        var wordsToStudy = (await vocabularyService.GetWordsToStudyAsync(userId, user.ActiveLanguage, wordsToFetch)).ToList();
        
        if (wordsToStudy.Count == 0)
            throw new Exception("No words to study");

        var sentenceTasks = wordsToStudy.Select(word => GenerateSentenceAsync(word, activeLanguage)).ToList();

        var sentences = await Task.WhenAll(sentenceTasks);
        
        var issueToken = await tokenService.GetIssueTokenAsync();

        return new SentencesResponse
        {
            Sentences = sentences,
            IssueToken = issueToken
        };
    }

    private List<StudyType> GetActiveStudyTypes(UserLanguage activeLanguage)
    {
        var activeTypes = new List<StudyType>();
        if (activeLanguage.Translation) activeTypes.Add(StudyType.Translation);
        if (activeLanguage.Listening) activeTypes.Add(StudyType.Listening);
        if (activeLanguage.Speaking) activeTypes.Add(StudyType.Speaking);
        return activeTypes;
    }

    private async Task<Sentence> GenerateSentenceAsync(Word word, UserLanguage activeLanguage)
    {
        // Step 1: Generate the sentence
        var sentenceText = await chatGptService.GenerateSentenceAsync(word.Word, activeLanguage.Language, activeLanguage.Country);

        // Step 2: Translate the sentence
        var translatedText = translationService.TranslateSentenceAsync(sentenceText, activeLanguage.Language);
        
        // Create and return the Sentence object
        return new Sentence
        {
            Original = sentenceText,
            Translation = await translatedText,
            Word = word.Word,
            Language = activeLanguage.Language
        };
    }
}