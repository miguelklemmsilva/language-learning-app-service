using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;

namespace LanguageLearningAppService.Lambda;

public class APIGatewayResponseManager
{
    public static APIGatewayHttpApiV2ProxyResponse ToAPIGatewaySuccessResponse<T>(T response)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(response, typeof(T), CustomJsonSerializerContext.Default)
        };
    }
}