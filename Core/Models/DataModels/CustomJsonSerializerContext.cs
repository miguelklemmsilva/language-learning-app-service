using System.Text.Json.Serialization;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Core.Models.ApiModels;
using Core.Models.DataTransferModels;

namespace Core.Models.DataModels;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
[JsonSerializable(typeof(UpdateUserRequest))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(UserLanguage))]
[JsonSerializable(typeof(RemoveUserLanguageResponse))]
[JsonSerializable(typeof(IEnumerable<UserLanguage>))]
[JsonSerializable(typeof(IEnumerable<string>))]
[JsonSerializable(typeof(IEnumerable<Word>))]
[JsonSerializable(typeof(IHttpResult))]
[JsonSerializable(typeof(HttpResults))]
[JsonSerializable(typeof(UserLanguageRequest))]
[JsonSerializable(typeof(AddVocabularyRequest))]
[JsonSerializable(typeof(ChatGptRequest))]
[JsonSerializable(typeof(ChatGptResponse))]
[JsonSerializable(typeof(SentencesResponse))]
public partial class CustomJsonSerializerContext : JsonSerializerContext
{
}