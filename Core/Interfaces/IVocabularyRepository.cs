using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IVocabularyRepository
{
    Task<Vocabulary> UpdateVocabularyAsync(Vocabulary vocabulary);
    Task<IEnumerable<Vocabulary>> GetUserVocabularyAsync(string userId, string language);
    Task RemoveVocabularyAsync(string userId, string languageWord);
    Task<Vocabulary> GetVocabularyAsync(string userId, string languageWord);
}