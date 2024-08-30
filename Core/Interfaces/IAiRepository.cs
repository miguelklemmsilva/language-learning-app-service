using Azure.AI.Translation.Text;

namespace Core.Interfaces;

public interface IAiRepository
{
    Task<string> GenerateSentenceAsync(string word, string language, string country);
    Task<TranslatedTextItem?> TranslateSentenceAsync(string sentence, string sourceLanguage);
}