using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models.ApiModels;

namespace LanguageLearningAppService.Tests.TestRepositories;

internal class TestChatGptRepository : IChatGptRepository
{
    public Task<ChatGptResponse> SendChatGptRequestAsync(ChatGptRequest request)
    {
        // If the request is for verifying a sentence, detect it via the evaluation prompt.
        if (request.Messages.Any(m => m.Content.Contains("Evaluate whether")))
        {
            var jsonResponse = "{\"isCorrect\":true, \"explanation\":\"\"}";
            var response = new ChatGptResponse
            {
                Choices =
                [
                    new ChatGptChoice
                    {
                        Message = new ChatGptMessage
                        {
                            Content = jsonResponse,
                            Role = "system"
                        }
                    }
                ]
            };
            return Task.FromResult(response);
        }

        // For generating a sentence, use the last user message (which is the input word)
        // and simulate a generated sentence.
        var lastUserMessage = request.Messages.Last().Content;
        var responseContent = "Sentence for " + lastUserMessage;
            
        var defaultResponse = new ChatGptResponse
        {
            Choices =
            [
                new ChatGptChoice
                {
                    Message = new ChatGptMessage
                    {
                        Content = responseContent,
                        Role = "assistant"
                    }
                }
            ]
        };

        return Task.FromResult(defaultResponse);
    }
}