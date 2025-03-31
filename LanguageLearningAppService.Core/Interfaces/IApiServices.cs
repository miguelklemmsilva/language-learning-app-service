using Azure.AI.Translation.Text;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Interfaces;

public interface ITokenRepository
{
    Task<string> GetIssueTokenAsync();
}

public interface ITranslationService
{
    Task<TranslatedTextItem?> TranslateSentenceAsync(string sentence, Language sourceLanguage);
}

public interface IChatGptService
{
    Task<string> GenerateSentenceAsync(string word, Language language, string country);
    Task<VerifySentenceResponse> VerifySentenceAsync(VerifySentenceRequest request);
}
