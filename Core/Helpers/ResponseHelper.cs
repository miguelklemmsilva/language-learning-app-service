using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Core.Models.DataModels;

namespace Core.Helpers
{
    public static class ResponseHelper
    {
        private static readonly Dictionary<string, string> CommonHeaders = new()
        {
            { "Content-Type", "application/json" },
            { "Access-Control-Allow-Origin", "*" },
            { "Access-Control-Allow-Methods", "GET,POST,OPTIONS" },
            { "Access-Control-Allow-Headers", "Content-Type" }
        };

        public static APIGatewayHttpApiV2ProxyResponse CreateSuccessResponse<T>(T body, JsonTypeInfo<T> jsonTypeInfo)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(body, jsonTypeInfo),
                Headers = CommonHeaders
            };
        }

        public static APIGatewayHttpApiV2ProxyResponse CreateErrorResponse(string message, int statusCode = 500)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = statusCode,
                Body = JsonSerializer.Serialize(new { error = message }),
                Headers = CommonHeaders
            };
        }
    }
}