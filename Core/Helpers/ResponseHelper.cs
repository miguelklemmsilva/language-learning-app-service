using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Annotations.APIGateway;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Helpers;

public static class ResponseHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly Dictionary<string, string> CommonHeaders = new()
    {
        { "Content-Type", "application/json" },
        { "Access-Control-Allow-Origin", "*" },
        { "Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS" },
        { "Access-Control-Allow-Headers", "Content-Type,Authorization" }
    };

    public static IHttpResult CreateSuccessResponse<T>(T body)
    {
        var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
        var result = HttpResults.Ok(jsonBody);

        foreach (var header in CommonHeaders)
        {
            result.AddHeader(header.Key, header.Value);
        }

        return HttpResults.Ok(jsonBody);
    }

    public static IHttpResult CreateErrorResponse(string message)
    {
        var errorBody = JsonSerializer.Serialize(new { error = message }, JsonOptions);
        var result = HttpResults.BadRequest(errorBody);

        foreach (var header in CommonHeaders)
        {
            result.AddHeader(header.Key, header.Value);
        }

        return result;
    }
}