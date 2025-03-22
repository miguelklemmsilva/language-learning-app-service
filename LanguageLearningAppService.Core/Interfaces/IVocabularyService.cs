using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Interfaces;

public interface IVocabularyService
{
    Task<IEnumerable<string>> AddVocabularyAsync(string userId, AddVocabularyRequest request);
    Task<IEnumerable<Word>> GetVocabularyAsync(string userId, string language);
    Task RemoveVocabularyAsync(string userId, string language, string word);
    Task<Vocabulary> UpdateVocabularyAsync(Vocabulary vocabulary);
    Task<IEnumerable<Word>> GetWordsToStudyAsync(string userId, string language, int count);
    Task FinishLessonAsync(string userId, FinishLessonRequest finishLessonRequest, string language);
}