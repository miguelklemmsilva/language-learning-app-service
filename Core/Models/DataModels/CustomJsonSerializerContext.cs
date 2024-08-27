using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;
using Core.Models.DataTransferModels;

namespace Core.Models.DataModels;

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
public partial class CustomJsonSerializerContext : JsonSerializerContext
{
}