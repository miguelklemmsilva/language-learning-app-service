using System.Text.RegularExpressions;
using Core.Helpers;
using Core.Interfaces;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Services;

public partial class VocabularyService(
    IVocabularyRepository vocabularyRepository,
    IAllowedVocabularyService allowedVocabularyService) : IVocabularyService
{
    private async Task<bool> IsVocabularyAllowedAsync(Language language, string word)
    {
        if (language == Language.Japanese)
            return true;

        return await allowedVocabularyService.IsVocabularyAllowedAsync(language, word);
    }

    private static string DecodeUnicodeEscapes(string input)
    {
        return MyRegex().Replace(input, match => ((char)Convert.ToInt32(match.Groups[1].Value, 16)).ToString());
    }

    public async Task<IEnumerable<string>> AddVocabularyAsync(string userId, AddVocabularyRequest request)
    {
        var currentVocabulary = (await GetVocabularyAsync(userId, request.Language)).ToList();
        var newWords = new List<string>();

        var decodedVocabulary = request.Vocabulary.Select(DecodeUnicodeEscapes).ToList();

        foreach (var word in decodedVocabulary)
        {
            try
            {
                if (!await IsVocabularyAllowedAsync(request.Language, word))
                    continue;

                if (currentVocabulary.Any(v =>
                        v.Language == request.Language &&
                        v.Word.Equals(word, StringComparison.OrdinalIgnoreCase)))
                    continue;

                await vocabularyRepository.UpdateVocabularyAsync(new Vocabulary
                {
                    UserId = userId,
                    Language = request.Language,
                    Word = word,
                    LastPracticed = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
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

    public async Task<IEnumerable<Word>> GetVocabularyAsync(string userId, Language language)
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
                Category = v.Category,
                MinutesUntilDue = minutesUntilDue,
                LastSeen = lastSeen
            };
        });
    }

    public async Task<Vocabulary> UpdateVocabularyAsync(Vocabulary vocabulary)
    {
        ArgumentNullException.ThrowIfNull(vocabulary);

        var existingVocabulary =
            await vocabularyRepository.GetUserVocabularyAsync(vocabulary.UserId!, vocabulary.Language);

        ArgumentNullException.ThrowIfNull(existingVocabulary);

        return await vocabularyRepository.UpdateVocabularyAsync(vocabulary);
    }

    public async Task RemoveVocabularyAsync(string userId, Language language, string word)
    {
        var existingVocabulary =
            await vocabularyRepository.GetVocabularyAsync(userId, language, word);

        ArgumentNullException.ThrowIfNull(existingVocabulary);

        await vocabularyRepository.RemoveVocabularyAsync(userId, language, word);
    }

    public async Task<IEnumerable<Word>> GetWordsToStudyAsync(string userId, Language language, int count)
    {
        var allVocabulary = await GetVocabularyAsync(userId, language);
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var wordsToStudy = allVocabulary
            .Where(v => v.MinutesUntilDue <= 0)
            .OrderBy(v => v.LastPracticed)
            .Take(count);

        return wordsToStudy;
    }

    public async Task FinishLessonAsync(string userId, FinishLessonRequest finishLessonRequest, Language language)
    {
        var groupedSentences = finishLessonRequest.Sentences.GroupBy(s => s.Word);

        foreach (var group in groupedSentences)
        {
            var word = group.Key;
            var wordScore = CalculateWordScore(group);

            var vocabulary = await vocabularyRepository.GetVocabularyAsync(userId, language, word);
            // Update existing vocabulary
            vocabulary.BoxNumber = Math.Max(1, Math.Min(9, vocabulary.BoxNumber + wordScore));
            vocabulary.LastPracticed = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            await vocabularyRepository.UpdateVocabularyAsync(vocabulary);
        }
    }

    private int CalculateWordScore(IGrouping<string, FinishLessonSentence> sentenceGroup)
    {
        var wordScore = 0;

        foreach (var sentence in sentenceGroup)
        {
            wordScore += sentence.Type switch
            {
                "translation" => sentence.Mistakes switch
                {
                    0 => 3,
                    1 => 2,
                    2 => 0,
                    3 => -1,
                    _ => -2
                },
                "listening" => sentence.Mistakes switch
                {
                    0 => 1,
                    1 => 1,
                    2 => 0,
                    3 => -1,
                    _ => -2
                },
                _ => 1
            };
        }

        return wordScore;
    }

    [GeneratedRegex(@"\\u([0-9A-Fa-f]{4})")]
    private static partial Regex MyRegex();
}