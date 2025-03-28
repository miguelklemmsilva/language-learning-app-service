using Azure.AI.Translation.Text;
using Core.Interfaces;

namespace LanguageLearningAppService.Tests.TestRepositories;

internal class TestTranslationRepository : ITranslationRepository
{
    public Task<TranslatedTextItem?> TranslateSentenceAsync(string sentence, string sourceLanguageCode)
    {
        throw new NotImplementedException();
    }
}