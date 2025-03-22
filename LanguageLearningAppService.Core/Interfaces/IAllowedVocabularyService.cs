using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IAllowedVocabularyService
{
    Task<bool> IsVocabularyAllowedAsync(string language, string word);
    Task<IEnumerable<Category>> GetWordsByCategoryAsync(string language);
}