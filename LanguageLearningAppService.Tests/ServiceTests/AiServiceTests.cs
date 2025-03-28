using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageLearningAppService.Tests.ServiceTests;

[Collection("DynamoDb Collection")]
public class AiServiceTests(DynamoDbFixture.DynamoDbFixture fixture)
{
    private readonly IAiService _aiService = fixture.ServiceProvider.GetRequiredService<IAiService>();
    private readonly IUserService _userService = fixture.ServiceProvider.GetRequiredService<IUserService>();

    [Fact]
    public async Task NoActiveLanguage_ShouldThrowException()
    {
        var user = UserServiceTests.CreateTestUser();

        await _userService.CreateUserAsync(user);

        Action act = () => _aiService.GenerateSentencesAsync(user.UserId).Wait();

        Assert.Throws<AggregateException>(act);
    }
}