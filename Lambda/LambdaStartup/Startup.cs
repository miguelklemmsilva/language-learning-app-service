using System.Net.Http.Headers;
using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Annotations;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using AWS.Services;
using Infrastructure.Repositories;
using Core.Interfaces;
using Core.Models.DataModels;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lambda.LambdaStartup;

[LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        IAmazonSecretsManager secretsManager = new AmazonSecretsManagerClient(RegionEndpoint.EUWest2);
        
        services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(RegionEndpoint.EUWest2));
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IUserLanguageRepository, UserLanguageRepository>();
        services.AddSingleton<IUserLanguageService, UserLanguageService>();
        services.AddSingleton<IVocabularyRepository, VocabularyRepository>();
        services.AddSingleton<IVocabularyService, VocabularyService>();
        services.AddSingleton<IAllowedVocabularyRepository, AllowedVocabularyRepository>();
        services.AddSingleton<IAllowedVocabularyService, AllowedVocabularyService>();
        services.AddHttpClient<IAiRepository, AiRepository>(client =>
        {
            client.BaseAddress = new Uri("https://api.openai.com");
            
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var key = secretsManager.GetSecretValueAsync(new GetSecretValueRequest
            {
                SecretId = "ChatGptKey"
            }).Result.SecretString;
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");
        });
        services.AddSingleton<IAiRepository, AiRepository>();
        services.AddSingleton<IAiService, AiService>();
    }
}