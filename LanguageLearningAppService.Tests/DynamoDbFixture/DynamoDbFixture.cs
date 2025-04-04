using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Core.Interfaces;
using Core.Services;
using LanguageLearningAppService.Infrastructure.Repositories;
using LanguageLearningAppService.Tests.TestRepositories;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageLearningAppService.Tests.DynamoDbFixture;

public class DynamoDbFixture : IDisposable
{
    public ServiceProvider ServiceProvider { get; }
    private TableManager TableManager { get; }

    public DynamoDbFixture()
    {
        var services = new ServiceCollection();
        
        var config = new AmazonDynamoDBConfig
        {
            ServiceURL = "http://localhost:8000",
            UseHttp = true
                
        };
        var client = new AmazonDynamoDBClient(config);
        TableManager = new TableManager(client);
        
        services.AddSingleton<IAmazonDynamoDB>(client);

        // Configure the DynamoDB client for local testing.
        services.AddSingleton<IDynamoDBContext>(_ =>
        {
            var context = new DynamoDBContextBuilder().WithDynamoDBClient(() => client).Build();
            
            TableManager.CreateTablesAsync().GetAwaiter().GetResult();

            return context;
        });

        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IUserLanguageRepository, UserLanguageRepository>();
        services.AddSingleton<IVocabularyRepository, VocabularyRepository>();
        services.AddSingleton<IAllowedVocabularyRepository, AllowedVocabularyRepository>();
        services.AddSingleton<ITokenRepository, TestTokenRepository>();
        services.AddSingleton<IChatGptRepository, TestChatGptRepository>();
        services.AddSingleton<ITranslationRepository, TestTranslationRepository>();

        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IUserLanguageService, UserLanguageService>();
        services.AddSingleton<IVocabularyService, VocabularyService>();
        services.AddSingleton<IAllowedVocabularyService, AllowedVocabularyService>();
        services.AddSingleton<IChatGptService, ChatGptService>();
        services.AddSingleton<ITranslationService, TranslationService>();
        services.AddSingleton<IAiService, AiService>();


        // Build the provider.
        ServiceProvider = services.BuildServiceProvider();
    }

    public async void Dispose()
    {
        TableManager.DeleteTables().Wait();
        await ServiceProvider.DisposeAsync();
    }
}

[CollectionDefinition("DynamoDb Collection")]
public class DynamoDbCollection : ICollectionFixture<DynamoDbFixture>;