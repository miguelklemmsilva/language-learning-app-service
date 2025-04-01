using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Models.ApiModels;
using Core.Models.DataTransferModels;

namespace Core;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(VerifySentenceResponse))]
public partial class ApiJsonSerializerContext : JsonSerializerContext;