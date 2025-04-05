using System.Net.Http.Headers;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Core.Interfaces;
using Core.Services;
using LanguageLearningAppService.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Azure;
using Azure.AI.Translation.Text;

namespace LanguageLearningAppService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration? configuration = null)
    {
        // Determine if a local DynamoDB endpoint is configured
        string? dynamoServiceUrl = configuration?["DynamoDb:ServiceUrl"];
        IAmazonDynamoDB dynamoClient;
            
        if (!string.IsNullOrEmpty(dynamoServiceUrl))
        {
            // Use local DynamoDB settings
            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = dynamoServiceUrl,
                UseHttp = true
            };
            dynamoClient = new AmazonDynamoDBClient(config);
        }
        else
        {
            // Use production settings
            dynamoClient = new AmazonDynamoDBClient(RegionEndpoint.EUWest2);
        }

        services.AddSingleton(dynamoClient);

        // Create DynamoDBContext using a builder
        services.AddSingleton<IDynamoDBContext>(sp =>
        {
            return new DynamoDBContextBuilder()
                .WithDynamoDBClient(() => dynamoClient)
                .Build();
        });

        // Register your repositories and services
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
        services.AddSingleton<ITokenRepository, TokenRepository>();
        services.AddSingleton<ITranslationRepository, TranslationRepository>();

        services.AddHttpClient<IChatGptRepository, ChatGptRepository>(ConfigureChatGptHttpClient);
        services.AddSingleton(_ => CreateTextTranslationClient());
        services.AddHttpClient<ITokenRepository, TokenRepository>(ConfigureSpeechServiceHttpClient);
        services.AddSingleton<IAiService, AiService>();
        
        return services;
    }

    private static void ConfigureChatGptHttpClient(HttpClient client)
    {
        var chatGptKey = Environment.GetEnvironmentVariable("CHAT_GPT_KEY");
        if (string.IsNullOrEmpty(chatGptKey))
            throw new InvalidOperationException("Chat GPT key is missing");

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
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
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