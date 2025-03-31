using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Interfaces;

public interface IVocabularyService
{
    Task<IEnumerable<string>> AddVocabularyAsync(string userId, AddVocabularyRequest request);
    Task<IEnumerable<Word>> GetVocabularyAsync(string userId, Language language);
    Task RemoveVocabularyAsync(string userId, Language language, string word);
    Task<Vocabulary> UpdateVocabularyAsync(Vocabulary vocabulary);
    Task<IEnumerable<Word>> GetWordsToStudyAsync(string userId, Language language, int count);
    Task FinishLessonAsync(string userId, FinishLessonRequest finishLessonRequest, Language language);
}