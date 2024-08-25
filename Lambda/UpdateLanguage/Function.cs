using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Serialization.SystemTextJson;
using AWS.Repositories;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;
using Core.Services;

namespace Lambda.UpdateLanguage;

public class Function
{
    private static readonly AmazonDynamoDBClient DynamoDbClient = new(RegionEndpoint.EUWest2);
    
    private static readonly Dictionary<string, string> CommonHeaders = new()
    {
        { "Content-Type", "application/json" },
        { "Access-Control-Allow-Origin", "*" },
        { "Access-Control-Allow-Methods", "GET, POST, OPTIONS" },
        { "Access-Control-Allow-Headers", "Content-Type" }
    };

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
        if (apigProxyEvent.RequestContext.Http.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string>(CommonHeaders)
                {
                    { "Access-Control-Allow-Headers", "Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token" },
                    { "Access-Control-Allow-Methods", "GET,POST,OPTIONS" }
                }
            };
        }
        
        
        string authToken = apigProxyEvent.Headers.TryGetValue("Authorization", out var header)
            ? header
            : string.Empty;

        if (string.IsNullOrEmpty(authToken))
            return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = "Authorization token is missing",
                StatusCode = 400,
                Headers = CommonHeaders
            };

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = tokenHandler.ReadJwtToken(authToken);

        var username = jwtSecurityToken.Claims.First(claim => claim.Type == "cognito:username").Value;

        if (apigProxyEvent.Body == null)
            return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = "Request body is missing",
                StatusCode = 400,
                Headers = CommonHeaders
            };

        // Parse the request body
        UpdateLanguageRequest updateRequest;
        try
        {
            updateRequest = JsonSerializer.Deserialize(apigProxyEvent.Body,
                LambdaFunctionJsonSerializerContext.Default.UpdateLanguageRequest)!;
        }
        catch (JsonException)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = "Invalid request body",
                StatusCode = 400,
                Headers = CommonHeaders
            };
        }

        var userLanguage = new UserLanguage
        {
            UserId = username,
            Language = updateRequest.Language,
            Country = updateRequest.Country,
            Translation = updateRequest.Translation,
            Listening = updateRequest.Listening,
            Speaking = updateRequest.Speaking
        };

        UserLanguageRepository userLanguageRepository = new(DynamoDbClient);
        UserRepository userRepository = new(DynamoDbClient);

        UserService userService = new(userRepository);
        var userLanguageService = new UserLanguageService(userLanguageRepository, userService);

        try
        {
            await userLanguageService.UpdateUserLanguageAsync(userLanguage);

            return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = JsonSerializer.Serialize(userLanguage,
                    LambdaFunctionJsonSerializerContext.Default.UserLanguage),
                StatusCode = 200,
                Headers = CommonHeaders
            };
        }
        catch (Exception ex)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = $"Error updating user language: {ex.Message}",
                StatusCode = 500,
                Headers = CommonHeaders
            };
        }
    }
}

[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
[JsonSerializable(typeof(UserLanguage))]
[JsonSerializable(typeof(UpdateLanguageRequest))]
public partial class LambdaFunctionJsonSerializerContext : JsonSerializerContext
{
    // By using this partial class derived from JsonSerializerContext, we can generate reflection-free JSON Serializer code at compile time
    // which can deserialize our class and properties. However, we must attribute this class to tell it what types to generate serialization code for.
    // See https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-source-generation
}