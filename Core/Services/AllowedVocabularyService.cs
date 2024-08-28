using Core.Interfaces;

namespace Core.Services;

public class AllowedVocabularyService(IAllowedVocabularyRepository allowedVocabularyRepository) : IAllowedVocabularyService
{
    public Task<bool> IsVocabularyAllowedAsync(string language, string word)
    {
        return allowedVocabularyRepository.IsVocabularyAllowedAsync(language, word);
    }
}