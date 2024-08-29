using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AWS.Services;
using Core.Interfaces;
using Core.Models.DataModels;

namespace Core.Services;

public enum StudyType
{
    Translation,
    Listening,
    Speaking
}

public class AiService(
    IAiRepository aiRepository,
    IUserService userService,
    IUserLanguageService userLanguageService,
    IVocabularyService vocabularyService)
    : IAiService
{
    public async Task<IEnumerable<Sentence>> GenerateSentencesAsync(string userId)
    {
        var user = await userService.GetUserAsync(userId);

        if (user.ActiveLanguage == null)
            throw new Exception("User has no active language");

        var activeLanguage = await userLanguageService.GetUserLanguageAsync(userId, user.ActiveLanguage);

        var activeStudyTypes = GetActiveStudyTypes(activeLanguage);

        if (activeStudyTypes.Count == 0)
            throw new Exception("User has no active exercises");

        var wordsToFetch = (int)Math.Ceiling(6.0 / activeStudyTypes.Count);
        var wordsToStudy = (await vocabularyService.GetWordsToStudyAsync(userId, user.ActiveLanguage, wordsToFetch)).ToList();
        
        if (wordsToStudy.Count == 0)
            throw new Exception("No words to study");

        var sentenceTasks = new List<Task<Sentence>>();

        foreach (var word in wordsToStudy)
        {
            sentenceTasks.AddRange(activeStudyTypes.Select(studyType =>
                GenerateSentenceAsync(word, studyType, activeLanguage)));
        }

        var sentences = await Task.WhenAll(sentenceTasks);

        return sentences;
    }

    private List<StudyType> GetActiveStudyTypes(UserLanguage activeLanguage)
    {
        var activeTypes = new List<StudyType>();
        if (activeLanguage.Translation) activeTypes.Add(StudyType.Translation);
        if (activeLanguage.Listening) activeTypes.Add(StudyType.Listening);
        if (activeLanguage.Speaking) activeTypes.Add(StudyType.Speaking);
        return activeTypes;
    }

    private async Task<Sentence> GenerateSentenceAsync(Word word, StudyType studyType, UserLanguage activeLanguage)
    {
        // Step 1: Generate the sentence
        var sentenceText = await aiRepository.GenerateSentenceAsync(word.Word, activeLanguage.Language, activeLanguage.Country);

        // // Step 2: Translate the sentence
        // var translatedText = aiRepository.TranslateSentenceAsync(sentenceText, activeLanguage.Language, "en");
        //
        // // Step 3: Generate voice
        // var voiceData = aiRepository.GenerateVoiceAsync(sentenceText, activeLanguage.Language);

        // Create and return the Sentence object
        return new Sentence
        {
            Original = sentenceText,
            // Translation = translatedText,
            Word = word.Word,
            Type = studyType.ToString().ToLower(),
            // Voice = voiceData,
            Language = activeLanguage.Language
        };
    }
}