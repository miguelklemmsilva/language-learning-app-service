using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Serialization.SystemTextJson;
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

    private static readonly HttpResultSerializationOptions HttpResultSerializationOptions =
        new()
        {
            Format = HttpResultSerializationOptions.ProtocolFormat.RestApi,
            Version = HttpResultSerializationOptions.ProtocolVersion.V2,
            Serializer = new SourceGeneratorLambdaJsonSerializer<CustomJsonSerializerContext>()
        };

    public static IHttpResult CreateSuccessResponse<T>(T body)
    {
        // Use the custom serializer context to serialize the response
        var result = HttpResults.Ok(body);

        // Add custom headers
        foreach (var header in CommonHeaders)
        {
            result.AddHeader(header.Key, header.Value);
        }

        result.Serialize(HttpResultSerializationOptions);

        return result;
    }

    public static IHttpResult CreateErrorResponse(string message)
    {
        // Serialize the error message with the custom serializer context
        var errorBody = new Dictionary<string, string> { { "message", message } };
        var result = HttpResults.BadRequest(errorBody);

        // Add custom headers
        foreach (var header in CommonHeaders)
        {
            result.AddHeader(header.Key, header.Value);
        }

        result.Serialize(HttpResultSerializationOptions);

        return result;
    }
}