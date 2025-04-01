using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;
using Core.Models.ApiModels;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace LanguageLearningAppService.Lambda;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
[JsonSerializable(typeof(UpdateUserRequest))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(UserResponse))]
[JsonSerializable(typeof(UserLanguage))]
[JsonSerializable(typeof(RemoveUserLanguageResponse))]
[JsonSerializable(typeof(IEnumerable<UserLanguage>))]
[JsonSerializable(typeof(IEnumerable<string>))]
[JsonSerializable(typeof(IEnumerable<Word>))]
[JsonSerializable(typeof(UserLanguageRequest))]
[JsonSerializable(typeof(AddVocabularyRequest))]
[JsonSerializable(typeof(ChatGptRequest))]
[JsonSerializable(typeof(ChatGptResponse))]
[JsonSerializable(typeof(SentencesResponse))]
[JsonSerializable(typeof(VerifySentenceResponse))]
[JsonSerializable(typeof(VerifySentenceRequest))]
[JsonSerializable(typeof(FinishLessonRequest))]
[JsonSerializable(typeof(JsonObject))]
[JsonSerializable(typeof(IEnumerable<Category>))]
public partial class CustomJsonSerializerContext : JsonSerializerContext;