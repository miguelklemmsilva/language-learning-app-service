using System.Net.Http.Headers;
using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Annotations;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using AWS.Services;
using Azure;
using Azure.AI.Translation.Text;
using Infrastructure.Repositories;
using Core.Interfaces;
using Core.Models.DataModels;
using Core.Services;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.DependencyInjection;

namespace Lambda.LambdaStartup;

[LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IAmazonSecretsManager>(_ => new AmazonSecretsManagerClient(RegionEndpoint.EUWest2));
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
        services.AddSingleton<ISpeechService, SpeechService>();

        services.AddHttpClient<IChatGptService, ChatGptService>((provider, client) =>
        {
            ConfigureHttpClientAsync(provider, client).GetAwaiter().GetResult();
        });

        services.AddSingleton(provider =>
        {
            var secretsManager = provider.GetRequiredService<IAmazonSecretsManager>();
            return CreateTextTranslationClientAsync(secretsManager).GetAwaiter().GetResult();
        });

        services.AddSingleton(provider =>
        {
            var secretsManager = provider.GetRequiredService<IAmazonSecretsManager>();
            return CreateSpeechConfigAsync(secretsManager).GetAwaiter().GetResult();
        });

        services.AddSingleton<IAiRepository, AiRepository>();
        services.AddSingleton<IAiService, AiService>();
    }

    private static async Task ConfigureHttpClientAsync(IServiceProvider provider, HttpClient client)
    {
        client.BaseAddress = new Uri("https://api.openai.com");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var secretsManager = provider.GetRequiredService<IAmazonSecretsManager>();
        var key = await GetSecretAsync(secretsManager, "ChatGptKey");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");
    }

    private static async Task<TextTranslationClient> CreateTextTranslationClientAsync(
        IAmazonSecretsManager secretsManager)
    {
        var translatorKey = await GetSecretAsync(secretsManager, "TranslatorKey");
        const string translatorRegion = "uksouth";

        var credential = new AzureKeyCredential(translatorKey);
        return new TextTranslationClient(credential, translatorRegion);
    }

    private static async Task<SpeechConfig> CreateSpeechConfigAsync(IAmazonSecretsManager secretsManager)
    {
        var speechKey = await GetSecretAsync(secretsManager, "SpeechKey");
        const string speechRegion = "uksouth";

        return SpeechConfig.FromSubscription(speechKey, speechRegion);
    }

    private static async Task<string> GetSecretAsync(IAmazonSecretsManager secretsManager, string secretId)
    {
        var response = await secretsManager.GetSecretValueAsync(new GetSecretValueRequest { SecretId = secretId });
        return response.SecretString;
    }
}
