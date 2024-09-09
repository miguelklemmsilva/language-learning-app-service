using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IAllowedVocabularyRepository
{
    Task<bool> IsVocabularyAllowedAsync(string language, string word);
    Task<IEnumerable<AllowedVocabulary>> GetWordsByCategoryAsync(string language);
}