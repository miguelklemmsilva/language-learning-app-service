using Core.Interfaces;
using Core.Models.DataModels;

namespace Core.Services;

public class AllowedVocabularyService(IAllowedVocabularyRepository allowedVocabularyRepository)
    : IAllowedVocabularyService
{
    public Task<bool> IsVocabularyAllowedAsync(string language, string word)
    {
        return allowedVocabularyRepository.IsVocabularyAllowedAsync(language, word);
    }

    public async Task<IEnumerable<Category>> GetWordsByCategoryAsync(string language)
    {
        var allWords = await allowedVocabularyRepository.GetWordsByCategoryAsync(language);

        return allWords
            .GroupBy(w => w.Category)
            .Select(g => new Category
            {
                Name = g.Key,
                Words = g.ToList()
            });
    }
}