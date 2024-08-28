namespace Core.Interfaces;

public interface IAllowedVocabularyRepository
{
    Task<bool> IsVocabularyAllowedAsync(string language, string word);
}