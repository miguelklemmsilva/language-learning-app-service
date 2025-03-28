using Azure.AI.Translation.Text;

namespace Core.Interfaces;

public interface ITranslationRepository
{
    public Task<TranslatedTextItem?> TranslateSentenceAsync(string sentence, string sourceLanguageCode);
}