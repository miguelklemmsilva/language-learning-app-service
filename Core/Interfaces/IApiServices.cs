using Azure.AI.Translation.Text;
using Microsoft.CognitiveServices.Speech;

namespace Core.Interfaces;

public interface ITokenService
{
    Task<string> GetIssueTokenAsync();
}

public interface ITranslationService
{
    Task<TranslatedTextItem?> TranslateSentenceAsync(string sentence, string sourceLanguage);
}

public interface IChatGptService
{
    Task<string> GenerateSentenceAsync(string word, string language, string country);
}
