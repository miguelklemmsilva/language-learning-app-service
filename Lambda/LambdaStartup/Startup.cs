using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Annotations;
using AWS.Repositories;
using AWS.Services;
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
    }
}