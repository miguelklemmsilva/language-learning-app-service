using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IVocabularyRepository
{
    Task<Vocabulary> UpdateVocabularyAsync(Vocabulary vocabulary);
    Task<IEnumerable<Vocabulary>> GetUserVocabularyAsync(string userId, Language language);
    Task RemoveVocabularyAsync(string userId, Language language, string word);
    Task<Vocabulary> GetVocabularyAsync(string userId, Language language, string word);
}