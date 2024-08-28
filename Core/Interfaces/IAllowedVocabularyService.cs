namespace Core.Interfaces;

public interface IAllowedVocabularyService
{
    public Task<bool> IsVocabularyAllowedAsync(string language, string word);
}