using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Services;

public interface IVocabularyService
{
    Task<IEnumerable<string>> AddVocabularyAsync(string userId, AddVocabularyRequest request);
    Task<IEnumerable<GetUserVocabularyResponse>> GetUserVocabularyAsync(string userId, string language);
    Task RemoveVocabularyAsync(string userId, string languageWord);
    Task<Vocabulary> UpdateVocabularyAsync(Vocabulary vocabulary);
}