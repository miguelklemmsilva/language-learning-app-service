using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IAllowedVocabularyService
{
    Task<bool> IsVocabularyAllowedAsync(Language language, string word);
    Task<IEnumerable<Category>> GetWordsByCategoryAsync(Language language);
}