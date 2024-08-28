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
            if (!await IsVocabularyAllowedAsync(request.Language, word)) continue;

            if (currentVocabulary.Any(v =>
                    v.Language.Equals(request.Language, StringComparison.OrdinalIgnoreCase) &&
                    v.Word.Equals(word, StringComparison.OrdinalIgnoreCase)))
                continue;

            await vocabularyRepository.UpdateVocabularyAsync(new Vocabulary
            {
                UserId = userId, Language = request.Language, Word = word,
                LastPracticed = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });
            newWords.Add(word);
        }

        return newWords;
    }

    public async Task<IEnumerable<GetVocabularyResponse>> GetVocabularyAsync(string userId, string language)
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

            return new GetVocabularyResponse
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
}