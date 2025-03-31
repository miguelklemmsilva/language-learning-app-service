using Core.Interfaces;
using Core.Models.DataModels;
using LanguageLearningAppService.Tests.TestBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageLearningAppService.Tests.ServiceTests;

[Collection("DynamoDb Collection")]
public class AiServiceTests(DynamoDbFixture.DynamoDbFixture fixture)
{
    private readonly IAiService _aiService = fixture.ServiceProvider.GetRequiredService<IAiService>();
    private readonly IUserService _userService = fixture.ServiceProvider.GetRequiredService<IUserService>();

    private readonly IUserLanguageService _userLanguageService =
        fixture.ServiceProvider.GetRequiredService<IUserLanguageService>();

    [Fact]
    public async Task No_Active_Language_Should_Throw_Exception()
    {
        var user = new UserBuilder().Build();

        await _userService.CreateUserAsync(user);

        var action = () => _aiService.GenerateSentencesAsync(user.UserId);

        await Assert.ThrowsAnyAsync<ArgumentNullException>(action);
    }

    [Fact]
    public async Task No_Active_Exercises_Should_Throw_Exception()
    {
        var user = new UserBuilder().Build();
        
        var userLanguage = new UserLanguage
        {
            UserId = user.UserId,
            Language = Language.Spanish,
            Country = "Spain",
            Translation = false,
            Listening = false,
            Speaking = false
        };

        var action = () => _userLanguageService.UpdateUserLanguageAsync(userLanguage);

        await Assert.ThrowsAnyAsync<NullReferenceException>(action);
    }
}