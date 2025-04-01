using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Helpers;

public static class ResponseHelper
{
    public static APIGatewayHttpApiV2ProxyResponse CreateSuccessResponse<T>(T body, Type type)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(body, type, CustomJsonSerializerContext.Default),
        };
    }

    public static APIGatewayHttpApiV2ProxyResponse CreateErrorResponse(string message, int statusCode = 500)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = statusCode,
            Body = JsonSerializer.Serialize(new Dictionary<string, string> { { "error", message } },
                typeof(Dictionary<string, string>), CustomJsonSerializerContext.Default),
        };
    }
}