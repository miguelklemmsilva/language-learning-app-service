using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Models.ApiModels;

namespace LanguageLearningAppService.Infrastructure;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(ChatGptRequest))]
[JsonSerializable(typeof(IEnumerable<ChatGptResponse>))]
public partial class ApiJsonSerializerContext : JsonSerializerContext;