using Core.Interfaces;

namespace LanguageLearningAppService.Infrastructure.Repositories;

public class TokenRepository(HttpClient httpClient) : ITokenRepository
{
    public async Task<string> GetIssueTokenAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/sts/v1.0/issueToken");
        var response = await httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }
}