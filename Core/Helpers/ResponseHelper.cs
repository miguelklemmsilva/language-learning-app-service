using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Annotations.APIGateway;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Helpers;

public static class ResponseHelper
{
    private static readonly Dictionary<string, string> CommonHeaders = new()
    {
        { "Content-Type", "application/json" },
        { "Access-Control-Allow-Origin", "*" },
        { "Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS" },
        { "Access-Control-Allow-Headers", "Content-Type,Authorization" }
    };

    public static APIGatewayProxyResponse CreateSuccessResponse<T>(T body)
    {
        var jsonBody = JsonSerializer.Serialize(body, typeof(T), CustomJsonSerializerContext.Default);
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Headers = CommonHeaders,
            Body = jsonBody,
        };
    }

    public static APIGatewayProxyResponse CreateErrorResponse(string message)
    {
        var errorBody = JsonSerializer.Serialize(new { error = message }, typeof(Dictionary<string, string>), CustomJsonSerializerContext.Default);
        return new APIGatewayProxyResponse
        {
            StatusCode = 400,
            Headers = CommonHeaders,
            Body = errorBody,
        };
    }
}