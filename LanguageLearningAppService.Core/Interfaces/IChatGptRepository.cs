using Core.Models.ApiModels;

namespace Core.Interfaces;

public interface IChatGptRepository
{
    Task<ChatGptResponse> SendChatGptRequestAsync(ChatGptRequest request);
}