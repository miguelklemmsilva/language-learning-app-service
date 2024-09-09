using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IAllowedVocabularyService
{
    public Task<bool> IsVocabularyAllowedAsync(string language, string word);
    public Task<List<AllowedVocabulary>> GetWordsByCategoryAsync(string language);
}