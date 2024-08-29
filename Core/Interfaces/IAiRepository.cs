namespace Core.Interfaces;

public interface IAiRepository
{
    Task<string> GenerateSentenceAsync(string word, string language, string country);
}