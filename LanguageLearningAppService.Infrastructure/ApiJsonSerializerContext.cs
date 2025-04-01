using System.Text.Json.Serialization;
using Core.Models.ApiModels;

namespace LanguageLearningAppService.Infrastructure;

[JsonSerializable(typeof(ChatGptRequest))]
[JsonSerializable(typeof(IEnumerable<ChatGptResponse>))]
public partial class ApiJsonSerializerContext : JsonSerializerContext;