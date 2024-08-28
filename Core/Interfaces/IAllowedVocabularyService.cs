namespace Core.Services;

public interface IAllowedVocabularyService
{
    public Task<bool> IsVocabularyAllowedAsync(string language, string word);
}