using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Helpers;

public static class ResponseHelper
{
    private static readonly Dictionary<string, string> CommonHeaders = new()
    {
        { "Content-Type", "application/json" },
        { "Access-Control-Allow-Origin", "*" },
        { "Access-Control-Allow-Methods", "GET,POST,OPTIONS" },
        { "Access-Control-Allow-Headers", "Content-Type" }
    };

    public static APIGatewayHttpApiV2ProxyResponse CreateSuccessResponse<T>(T body, Type type)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(body, type, LambdaFunctionJsonSerializerContext.Default),
            Headers = CommonHeaders
        };
    }

    public static APIGatewayHttpApiV2ProxyResponse CreateErrorResponse(string message, int statusCode = 500)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = statusCode,
            Body = JsonSerializer.Serialize(new Dictionary<string, string> { { "error", message } },
                typeof(Dictionary<string, string>), LambdaFunctionJsonSerializerContext.Default),
            Headers = CommonHeaders
        };
    }
}   

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
    )]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(UserLanguage))]
[JsonSerializable(typeof(RemoveUserLanguageResponse))]
[JsonSerializable(typeof(IEnumerable<UserLanguage>))]
[JsonSerializable(typeof(IEnumerable<string>))]
[JsonSerializable(typeof(IEnumerable<GetUserVocabularyResponse>))]
internal partial class LambdaFunctionJsonSerializerContext : JsonSerializerContext
{
}