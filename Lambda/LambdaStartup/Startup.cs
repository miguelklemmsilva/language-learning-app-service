using System.Net.Http.Headers;
using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Annotations;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
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
    private const string SecretName = "AppSecrets";

    public void ConfigureServices(IServiceCollection services)
    {
        var secretsManager = new AmazonSecretsManagerClient(RegionEndpoint.EUWest2);
        var secrets = GetSecretsAsync(secretsManager).GetAwaiter().GetResult();

        services.AddSingleton<IAmazonSecretsManager>(secretsManager);
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

        services.AddHttpClient<IChatGptService, ChatGptService>(client =>
        {
            ConfigureChatGptHttpClient(client, secrets);
        });

        services.AddSingleton(provider => CreateTextTranslationClient(secrets));

        services.AddHttpClient<ITokenService, TokenService>(client =>
        {
            ConfigureSpeechServiceHttpClient(client, secrets);
        });

        services.AddSingleton<IAiService, AiService>();
    }

    private static void ConfigureChatGptHttpClient(HttpClient client, Dictionary<string, string> secrets)
    {
        client.BaseAddress = new Uri("https://api.openai.com");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {secrets["chatGptKey"]}");
    }

    private static void ConfigureSpeechServiceHttpClient(HttpClient client, Dictionary<string, string> secrets)
    {
        client.BaseAddress = new Uri("https://uksouth.api.cognitive.microsoft.com");
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", secrets["speechKey"]);
    }

    private static TextTranslationClient CreateTextTranslationClient(Dictionary<string, string> secrets)
    {
        var translatorKey = secrets["translatorKey"];
        const string translatorRegion = "uksouth";

        var credential = new AzureKeyCredential(translatorKey);
        return new TextTranslationClient(credential, translatorRegion);
    }

    private static async Task<Dictionary<string, string>> GetSecretsAsync(IAmazonSecretsManager secretsManager)
    {
        var response = await secretsManager.GetSecretValueAsync(new GetSecretValueRequest { SecretId = SecretName });
        return JsonSerializer.Deserialize(response.SecretString,
                   CustomJsonSerializerContext.Default.DictionaryStringString) ??
               throw new Exception("Failed to retrieve secrets");
    }
}