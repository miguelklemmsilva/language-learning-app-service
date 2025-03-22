using System.Net.Http.Headers;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Annotations;
using Azure;
using Azure.AI.Translation.Text;
using Infrastructure.Repositories;
using Core.Interfaces;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lambda.LambdaStartup;

[LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(RegionEndpoint.EUWest2));
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IUserLanguageRepository, UserLanguageRepository>();
        services.AddSingleton<IUserLanguageService, UserLanguageService>();
        services.AddSingleton<IVocabularyRepository, VocabularyRepository>();
        services.AddSingleton<IVocabularyService, VocabularyService>();
        services.AddSingleton<IAllowedVocabularyRepository, AllowedVocabularyRepository>();
        services.AddSingleton<IAllowedVocabularyService, AllowedVocabularyService>();
        services.AddSingleton<IChatGptService, ChatGptService>();
        services.AddSingleton<ITranslationService, TranslationService>();
        services.AddSingleton<ITokenService, TokenService>();

        services.AddHttpClient<IChatGptService, ChatGptService>(ConfigureChatGptHttpClient);

        services.AddSingleton(_ => CreateTextTranslationClient());

        services.AddHttpClient<ITokenService, TokenService>(ConfigureSpeechServiceHttpClient);

        services.AddSingleton<IAiService, AiService>();
    }

    private static void ConfigureChatGptHttpClient(HttpClient client)
    {
        var chatGptKey = Environment.GetEnvironmentVariable("CHAT_GPT_KEY")!;

        client.BaseAddress = new Uri("https://api.openai.com");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {chatGptKey}");
    }

    private static void ConfigureSpeechServiceHttpClient(HttpClient client)
    {
        var speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
        if (string.IsNullOrEmpty(speechKey))
            throw new InvalidOperationException("Speech key is missing");

        client.BaseAddress = new Uri("https://uksouth.api.cognitive.microsoft.com");
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", speechKey);
    }

    private static TextTranslationClient CreateTextTranslationClient()
    {
        var translatorKey = Environment.GetEnvironmentVariable("TRANSLATOR_KEY");
        if (string.IsNullOrEmpty(translatorKey))
            throw new InvalidOperationException("Translator key is missing");

        const string translatorRegion = "uksouth";

        var credential = new AzureKeyCredential(translatorKey);
        return new TextTranslationClient(credential, translatorRegion);
    }
}