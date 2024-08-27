using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Annotations.APIGateway;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Helpers;

public static class ResponseHelper
{
    // You can reuse your existing headers
    private static readonly Dictionary<string, string> CommonHeaders = new()
    {
        { "Content-Type", "application/json" },
        { "Access-Control-Allow-Origin", "*" },
        { "Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS" },
        { "Access-Control-Allow-Headers", "Content-Type,Authorization" }
    };

    public static IHttpResult CreateSuccessResponse<T>(T body)
    {
        // Use the custom serializer context to serialize the response
        var jsonBody = JsonSerializer.Serialize(body, typeof(T), CustomJsonSerializerContext.Default);
        var result = HttpResults.Ok(jsonBody);

        // Add custom headers
        foreach (var header in CommonHeaders)
        {
            result.AddHeader(header.Key, header.Value);
        }

        return result;
    }

    public static IHttpResult CreateErrorResponse(string message)
    {
        // Serialize the error message with the custom serializer context
        var errorBody = JsonSerializer.Serialize(new Dictionary<string, string> { { "message", message } },
            typeof(Dictionary<string, string>), CustomJsonSerializerContext.Default);
        var result = HttpResults.BadRequest(errorBody);

        // Add custom headers
        foreach (var header in CommonHeaders)
        {
            result.AddHeader(header.Key, header.Value);
        }

        return result;
    }
}