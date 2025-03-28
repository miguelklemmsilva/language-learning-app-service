using Azure;
using Azure.AI.Translation.Text;
using Core.Interfaces;

namespace LanguageLearningAppService.Infrastructure.Repositories;

public class TranslationRepository(TextTranslationClient textTranslationClient) : ITranslationRepository
{
    public async Task<TranslatedTextItem?> TranslateSentenceAsync(string sentence, string sourceLanguageCode)
    {
        var options = new TextTranslationTranslateOptions(targetLanguages: ["en"], content: [sentence])
        {
            SourceLanguage = sourceLanguageCode,
            ProfanityAction = ProfanityAction.NoAction,
            IncludeAlignment = true
        };

        Response<IReadOnlyList<TranslatedTextItem>> response = await textTranslationClient.TranslateAsync(options);
        IReadOnlyList<TranslatedTextItem?> translations = response.Value;
        return translations.FirstOrDefault();
    }
}