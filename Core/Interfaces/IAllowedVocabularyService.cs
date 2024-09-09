using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IAllowedVocabularyService
{
    public Task<bool> IsVocabularyAllowedAsync(string language, string word);
    public Task<IEnumerable<AllowedVocabulary>> GetWordsByCategoryAsync(string language);
}