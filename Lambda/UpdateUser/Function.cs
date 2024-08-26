using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Serialization.SystemTextJson;
using AWS.Repositories;
using AWS.Services;
using Core.Helpers;
using Core.Interfaces;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;
using Core.Services;

namespace Lambda.UpdateUser;

public class Function
{
    private static readonly AmazonDynamoDBClient DynamoDbClient = new(RegionEndpoint.EUWest2);

    private static async Task Main()
    {
        Func<APIGatewayHttpApiV2ProxyRequest, ILambdaContext, Task<APIGatewayHttpApiV2ProxyResponse>> handler =
            FunctionHandler;
        await LambdaBootstrapBuilder.Create(handler,
                new SourceGeneratorLambdaJsonSerializer<LambdaFunctionJsonSerializerContext>())
            .Build()
            .RunAsync();
    }

    public static async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(
        APIGatewayHttpApiV2ProxyRequest apigProxyEvent, ILambdaContext context)
    {
        try
        {
            var username = AuthHelper.ParseToken(apigProxyEvent.Headers).CognitoUsername;

            var updateRequest = JsonSerializer.Deserialize(apigProxyEvent.Body,
                LambdaFunctionJsonSerializerContext.Default.UpdateUserRequest)!;

            var updateUserRequest = new UpdateUserRequest { ActiveLanguage = updateRequest.ActiveLanguage };

            IUserRepository userRepository = new UserRepository(DynamoDbClient);
            IUserService userService = new UserService(userRepository);

            var user = await userService.UpdateUserAsync(new User
                { UserId = username, ActiveLanguage = updateUserRequest.ActiveLanguage });

            return ResponseHelper.CreateSuccessResponse(user, typeof(User));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }
}

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
[JsonSerializable(typeof(UpdateUserRequest))]
public partial class LambdaFunctionJsonSerializerContext : JsonSerializerContext
{
    // By using this partial class derived from JsonSerializerContext, we can generate reflection-free JSON Serializer code at compile time
    // which can deserialize our class and properties. However, we must attribute this class to tell it what types to generate serialization code for.
    // See https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-source-generation
}