using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IAllowedVocabularyService
{
    Task<bool> IsVocabularyAllowedAsync(string language, string word);
    Task<IDictionary<string, IEnumerable<string>>> GetWordsByCategoryAsync(string language);
}