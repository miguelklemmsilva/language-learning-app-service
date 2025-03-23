using Core.Interfaces;
using Core.Models.DataModels;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageLearningAppService.Tests.ServiceTests;

[Collection("DynamoDb Collection")]
public class UserServiceTests(DynamoDbFixture.DynamoDbFixture fixture)
{
    private readonly IUserService _userService = fixture.ServiceProvider.GetRequiredService<IUserService>();

    private User CreateTestUser() => new()
    { 
        UserId = Guid.NewGuid().ToString(), 
        Email = "test@example.com" 
    };
    
    [Fact]
    public async Task Can_Create_And_Retrieve_User()
    {
        var user = CreateTestUser();
        
        // Act
        await _userService.CreateUserAsync(user);
        var retrievedUser = await _userService.GetUserAsync(user.UserId);

        // Assert
        Assert.Equal(user.UserId, retrievedUser.User.UserId);
        Assert.Equal(user.Email, retrievedUser.User.Email);
    }

    [Fact]
    public async Task Can_Update_User()
    {
        // Arrange
        var user = CreateTestUser();

        await _userService.CreateUserAsync(user);
        user.ActiveLanguage = "Spanish";
        
        // Act
        var updatedUser = await _userService.UpdateUserAsync(user);
        
        // Assert
        Assert.Equal(user.UserId, updatedUser.User.UserId);
        Assert.Equal(user.Email, updatedUser.User.Email);
        Assert.Equal(user.ActiveLanguage, updatedUser.User.ActiveLanguage);
    }

    [Fact]
    public async Task Can_Remove_Active_Language()
    {
        // Arrange
        var user = CreateTestUser();

        await _userService.CreateUserAsync(user);
        user.ActiveLanguage = "Spanish";
        var updatedUser = await _userService.UpdateUserAsync(user);

        updatedUser.User.ActiveLanguage = null;
        
        // Act
        var userWithoutActiveLanguage = await _userService.UpdateUserAsync(updatedUser.User);
        var persistedUser = await _userService.GetUserAsync(user.UserId); 
        
        // Assert
        Assert.Null(userWithoutActiveLanguage.User.ActiveLanguage);
        Assert.Null(persistedUser.User.ActiveLanguage);
    }
}