using Core.Interfaces;

namespace LanguageLearningAppService.Tests.TestRepositories;

internal class TestTokenRepository : ITokenRepository
{
    public Task<string> GetIssueTokenAsync()
    {
        return Task.Run(() => "test-token");
    }
}