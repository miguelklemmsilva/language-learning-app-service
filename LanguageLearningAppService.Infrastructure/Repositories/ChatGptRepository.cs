using System.Text;
using System.Text.Json;
using Core.Interfaces;
using Core.Models.ApiModels;
using Core.Models.DataModels;

namespace LanguageLearningAppService.Infrastructure.Repositories;

public class ChatGptRepository(HttpClient httpClient) : IChatGptRepository
{
    public async Task<ChatGptResponse> SendChatGptRequestAsync(ChatGptRequest request)
    {
        var requestJson = JsonSerializer.Serialize(request, CustomJsonSerializerContext.Default.ChatGptRequest);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/v1/chat/completions")
        {
            Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
        };

        var response = await httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var chatGptResponse =
            JsonSerializer.Deserialize(responseBody, CustomJsonSerializerContext.Default.ChatGptResponse);
        return chatGptResponse!;
    }
}