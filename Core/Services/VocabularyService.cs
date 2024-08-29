using AWS.Services;
using Core.Helpers;
using Core.Interfaces;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Services;

public class VocabularyService(
    IVocabularyRepository vocabularyRepository,
    IAllowedVocabularyService allowedVocabularyService) : IVocabularyService
{
    private async Task<bool> IsVocabularyAllowedAsync(string language, string word)
    {
        if (language.Equals("Japanese"))
            return true;
        
        return await allowedVocabularyService.IsVocabularyAllowedAsync(language, word);
    }
    
    public async Task<IEnumerable<string>> AddVocabularyAsync(string userId, AddVocabularyRequest request)
    {
        var currentVocabulary = (await GetVocabularyAsync(userId, request.Language)).ToList();
        var newWords = new List<string>();

        foreach (var word in request.Vocabulary)
        {
            try
            {
                if (!await IsVocabularyAllowedAsync(request.Language, word))
                    continue;

                if (currentVocabulary.Any(v =>
                        v.Language.Equals(request.Language, StringComparison.OrdinalIgnoreCase) &&
                        v.Word.Equals(word, StringComparison.OrdinalIgnoreCase)))
                    continue;

                await vocabularyRepository.UpdateVocabularyAsync(new Vocabulary
                {
                    UserId = userId,
                    Language = request.Language,
                    Word = word,
                    LastPracticed = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });
            
                newWords.Add(word);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing word '{word}': {ex.Message}");
            }

        }

        return newWords;
    }

    public async Task<IEnumerable<Word>> GetVocabularyAsync(string userId, string language)
    {
        var vocabularies = await vocabularyRepository.GetUserVocabularyAsync(userId, language);

        var now = DateTime.UtcNow;

        return vocabularies.Select(v =>
        {
            var lastPracticed = DateTimeOffset.FromUnixTimeSeconds(v.LastPracticed).UtcDateTime;
            var lastSeen = (long)(now - lastPracticed).TotalMinutes;

            var minutesUntilDue = v.BoxNumber switch
            {
                1 => 0,
                2 => Math.Max(10 - lastSeen, 0),
                3 => Math.Max(60 - lastSeen, 0),
                4 => Math.Max(1440 - lastSeen, 0),
                5 => Math.Max(4320 - lastSeen, 0),
                6 => Math.Max(10080 - lastSeen, 0),
                7 => Math.Max(20160 - lastSeen, 0),
                8 => Math.Max(40320 - lastSeen, 0),
                9 => Math.Max(80640 - lastSeen, 0),
                _ => 0
            };

            return new Word
            {
                UserId = v.UserId,
                Language = v.Language,
                Word = v.Word,
                LastPracticed = v.LastPracticed,
                BoxNumber = v.BoxNumber,
                MinutesUntilDue = minutesUntilDue,
                LastSeen = lastSeen
            };
        });
    }

    public async Task<Vocabulary> UpdateVocabularyAsync(Vocabulary vocabulary)
    {
        var existingVocabulary =
            await vocabularyRepository.GetUserVocabularyAsync(vocabulary.UserId, vocabulary.Language);

        if (existingVocabulary == null)
            throw new Exception("Vocabulary not found");

        return await vocabularyRepository.UpdateVocabularyAsync(vocabulary);
    }

    public async Task RemoveVocabularyAsync(string userId, string language, string word)
    {
        var existingVocabulary =
            await vocabularyRepository.GetVocabularyAsync(userId, language, word);

        if (existingVocabulary == null)
            throw new Exception("Vocabulary not found");

        await vocabularyRepository.RemoveVocabularyAsync(userId, language, word);
    }
    
    public async Task<IEnumerable<Word>> GetWordsToStudyAsync(string userId, string language, int count)
    {
        var allVocabulary = await GetVocabularyAsync(userId, language);
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var wordsToStudy = allVocabulary
            .Where(v => IsWordDueForStudy(v, now))
            .OrderBy(v => v.LastPracticed)
            .Take(count);
        
        return wordsToStudy;
    }

    private bool IsWordDueForStudy(Word vocabulary, long currentTime)
    {
        var lastPracticed = vocabulary.LastPracticed;
        var timeSinceLastPractice = currentTime - lastPracticed;

        return vocabulary.BoxNumber switch
        {
            1 => true, // Always due
            2 => timeSinceLastPractice >= 10 * 60, // 10 minutes
            3 => timeSinceLastPractice >= 60 * 60, // 1 hour
            4 => timeSinceLastPractice >= 24 * 60 * 60, // 1 day
            5 => timeSinceLastPractice >= 3 * 24 * 60 * 60, // 3 days
            6 => timeSinceLastPractice >= 7 * 24 * 60 * 60, // 7 days
            7 => timeSinceLastPractice >= 14 * 24 * 60 * 60, // 14 days
            8 => timeSinceLastPractice >= 28 * 24 * 60 * 60, // 28 days
            9 => timeSinceLastPractice >= 56 * 24 * 60 * 60, // 56 days
            _ => false
        };
    }
}