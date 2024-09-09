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

    public async Task<List<AllowedVocabulary>> GetWordsByCategoryAsync(string language)
    {
        return await allowedVocabularyRepository.GetWordsByCategoryAsync(language);
    }
}   