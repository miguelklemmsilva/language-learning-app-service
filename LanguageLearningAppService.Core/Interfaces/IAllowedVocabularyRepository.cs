using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IAllowedVocabularyRepository
{
    Task<bool> IsVocabularyAllowedAsync(Language language, string word);
    Task<IEnumerable<AllowedVocabulary>> GetWordsByCategoryAsync(Language language);
}