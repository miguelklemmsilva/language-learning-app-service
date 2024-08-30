using Azure.AI.Translation.Text;
using Core.Interfaces;
using Core.Models.DataModels;
using Microsoft.CognitiveServices.Speech;

namespace Infrastructure.Repositories;

public class AiRepository(
    IChatGptService chatGptService,
    ITranslationService translationService,
    ISpeechService speechService)
    : IAiRepository
{
    public async Task<string> GenerateSentenceAsync(string word, string language, string country)
    {
        return await chatGptService.GenerateSentenceAsync(word, language, country);
    }

    public async Task<TranslatedTextItem?> TranslateSentenceAsync(string sentence, string sourceLanguage)
    {
        return await translationService.TranslateSentenceAsync(sentence, sourceLanguage);
    }

    public async Task<SpeechSynthesisResult> SynthesizeSpeechAsync(string text, string country)
    {
        return await speechService.SynthesizeSpeechAsync(text, country);
    }
}