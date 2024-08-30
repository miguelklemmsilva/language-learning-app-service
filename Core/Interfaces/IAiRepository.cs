using Azure.AI.Translation.Text;
using Microsoft.CognitiveServices.Speech;

namespace Core.Interfaces;

public interface IAiRepository
{
    Task<string> GenerateSentenceAsync(string word, string language, string country);
    Task<TranslatedTextItem?> TranslateSentenceAsync(string sentence, string sourceLanguage);
    Task<SpeechSynthesisResult> SynthesizeSpeechAsync(string text, string country);
}