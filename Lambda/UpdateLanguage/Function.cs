using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace Lambda.UpdateLanguage;

public class Function
{
    private static readonly HttpClient client = new HttpClient();

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
        string authToken = apigProxyEvent.Headers.TryGetValue("Authorization", out var header)
            ? header
            : string.Empty;

        if (string.IsNullOrEmpty(authToken))
            return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = "Authorization token is missing",
                StatusCode = 400
            };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var jwtSecurityToken = tokenHandler.ReadJwtToken(authToken);

        var username = jwtSecurityToken.Claims.First(claim => claim.Type == "cognito:username").Value;
        var email = jwtSecurityToken.Claims.First(claim => claim.Type == "email").Value;
        
        Console.WriteLine(username);
        Console.WriteLine(email);

        var body = new Dictionary<string, string>
        {
            { "username", username },
            { "email", email }
        };

        return new APIGatewayHttpApiV2ProxyResponse
        {
            Body = JsonSerializer.Serialize(body, typeof(Dictionary<string, string>),
                LambdaFunctionJsonSerializerContext.Default),
            StatusCode = 200,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Origin", "*" },
                { "Access-Control-Allow-Methods", "GET, POST, OPTIONS" },
                { "Access-Control-Allow-Headers", "Content-Type" }
            }
        };
    }
}

[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
[JsonSerializable(typeof(Dictionary<string, string>))]
public partial class LambdaFunctionJsonSerializerContext : JsonSerializerContext
{
    // By using this partial class derived from JsonSerializerContext, we can generate reflection-free JSON Serializer code at compile time
    // which can deserialize our class and properties. However, we must attribute this class to tell it what types to generate serialization code for.
    // See https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-source-generation
}