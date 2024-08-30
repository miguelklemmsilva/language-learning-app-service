using Azure.AI.Translation.Text;
using Microsoft.CognitiveServices.Speech;

namespace Core.Interfaces;

public interface ISpeechService
{
    Task<SpeechSynthesisResult> SynthesizeSpeechAsync(string text, string country);
}

public interface ITranslationService
{
    Task<TranslatedTextItem?> TranslateSentenceAsync(string sentence, string sourceLanguage);
}

public interface IChatGptService
{
    Task<string> GenerateSentenceAsync(string word, string language, string country);
}
