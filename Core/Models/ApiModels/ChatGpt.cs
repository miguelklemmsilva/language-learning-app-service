using System.Text.Json.Serialization;

namespace Core.Models.ApiModels;

public class ChatGptRequest
{
    public required string Model { get; set; }
    public required ChatGptMessage[] Messages { get; set; }
    public double Temperature { get; set; } = 0.7;
    [JsonPropertyName("response_format")]
    public ResponseFormat? ResponseFormat { get; set; }
}

public class ResponseFormat
{
    public string Type { get; set; } = "json_schema";
    [JsonPropertyName("json_schema")]
    public JsonSchema? JsonSchema { get; set; }
}

public class JsonSchema
{
    public string Name { get; set; } = "ChatGptResponse";
    public bool Strict { get; set; } = true;
    public required Schema Schema { get; set; }
}

public class Schema
{
    public string Type { get; set; } = "object";
    public Dictionary<string, JsonSchemaProperty> Properties { get; set; } = new();
    public string[]? Required { get; set; }
}

public class JsonSchemaProperty
{
    public required string Type { get; set; }
}

public class ChatGptMessage
{
    public required string Role { get; set; }
    public required string Content { get; set; }
}

public class ChatGptResponse
{
    public required ChatGptChoice[] Choices { get; set; }
}

public class ChatGptChoice
{
    public required ChatGptMessage Message { get; set; }
}