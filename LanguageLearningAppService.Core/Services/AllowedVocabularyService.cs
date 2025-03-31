using Core.Interfaces;
using Core.Models.DataModels;

namespace Core.Services;

public class AllowedVocabularyService(IAllowedVocabularyRepository allowedVocabularyRepository)
    : IAllowedVocabularyService
{
    public async Task<bool> IsVocabularyAllowedAsync(Language language, string word)
    {
        if (string.IsNullOrEmpty(word)) return false;
        
        return await allowedVocabularyRepository.IsVocabularyAllowedAsync(language, word.ToLower());
    }

    public async Task<IEnumerable<Category>> GetWordsByCategoryAsync(Language language)
    {
        ArgumentNullException.ThrowIfNull(language);
        
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