using Core.Interfaces;
using Core.Models.ApiModels;

namespace LanguageLearningAppService.Tests.TestRepositories;

internal class TestChatGptRepository : IChatGptRepository
{
    public Task<ChatGptResponse> SendChatGptRequestAsync(ChatGptRequest request)
    {
        throw new NotImplementedException();
    }
}