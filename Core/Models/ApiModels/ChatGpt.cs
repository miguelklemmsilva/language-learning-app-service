namespace Core.Models.ApiModels;

public class ChatGptRequest
{
    public required string Model { get; set; }
    public required ChatGptMessage[] Messages { get; set; }
    public double Temperature { get; set; }
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