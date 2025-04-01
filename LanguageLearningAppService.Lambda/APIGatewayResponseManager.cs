using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;

namespace LanguageLearningAppService.Lambda;

public static class ApiGatewayResponseManager
{
    public static APIGatewayHttpApiV2ProxyResponse ToApiGatewaySuccessResponse<T>(T response)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(response, typeof(T), CustomJsonSerializerContext.Default)
        };
    }
}