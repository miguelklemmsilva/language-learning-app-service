using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Core.Interfaces;
using Core.Services;
using LanguageLearningAppService.Extensions;
using LanguageLearningAppService.Infrastructure.Repositories;
using LanguageLearningAppService.Tests.TestRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LanguageLearningAppService.Tests.DynamoDbFixture;

public class DynamoDbFixture : IDisposable
{
    public ServiceProvider ServiceProvider { get; }
    private TableManager TableManager { get; }

    public DynamoDbFixture()
    {
        var services = new ServiceCollection();
        // Build a configuration with the local DynamoDB endpoint
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "DynamoDb:ServiceUrl", "http://localhost:8000" }
            }!)
            .Build();

        services.AddCommonServices(configuration);
        
        // Remove the production HTTP client-based repository and add the test version
        services.RemoveAll<IChatGptRepository>();
        services.AddSingleton<IChatGptRepository, TestChatGptRepository>();

        // Remove the production token repository and add the test version
        services.RemoveAll<ITokenRepository>();
        services.AddSingleton<ITokenRepository, TestTokenRepository>();

        // Remove the production translation repository and add the test version
        services.RemoveAll<ITranslationRepository>();
        services.AddSingleton<ITranslationRepository, TestTranslationRepository>();

        ServiceProvider = services.BuildServiceProvider();


        var client = ServiceProvider.GetRequiredService<IAmazonDynamoDB>();
        TableManager = new TableManager(client);

        TableManager.CreateTablesAsync().Wait();
    }

    public async void Dispose()
    {
        TableManager.DeleteTables().Wait();
        await ServiceProvider.DisposeAsync();
    }
}

[CollectionDefinition("DynamoDb Collection")]
public class DynamoDbCollection : ICollectionFixture<DynamoDbFixture>;