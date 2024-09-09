using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IAllowedVocabularyService
{
    public Task<bool> IsVocabularyAllowedAsync(string language, string word);
    Task<IEnumerable<AllowedVocabulary>> GetAllowedVocabularyByLanguageAsync(string language);
}