using Core.Interfaces;
using Core.Models.DataModels;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageLearningAppService.Tests.RepositoryTests;

[Collection("DynamoDb Collection")]
public sealed class UserRepositoryTests(DynamoDbFixture.DynamoDbFixture fixture)
{
    private readonly IUserRepository _userRepository = fixture.ServiceProvider.GetRequiredService<IUserRepository>();

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
        await _userRepository.CreateUserAsync(user);
        var retrievedUser = await _userRepository.GetUserAsync(user.UserId);

        // Assert
        Assert.Equal(user.UserId, retrievedUser.UserId);
        Assert.Equal(user.Email, retrievedUser.Email);
    }

    [Fact]
    public async Task Can_Update_User()
    {
        // Arrange
        var user = CreateTestUser();

        await _userRepository.CreateUserAsync(user);
        user.ActiveLanguage = "Spanish";
        
        // Act
        var updatedUser = await _userRepository.UpdateUserAsync(user);
        
        // Assert
        Assert.Equal(user.UserId, updatedUser.UserId);
        Assert.Equal(user.Email, updatedUser.Email);
        Assert.Equal(user.ActiveLanguage, updatedUser.ActiveLanguage);
    }

    [Fact]
    public async Task Can_Remove_Active_Language()
    {
        // Arrange
        var user = CreateTestUser();

        await _userRepository.CreateUserAsync(user);
        user.ActiveLanguage = "Spanish";
        var updatedUser = await _userRepository.UpdateUserAsync(user);

        updatedUser.ActiveLanguage = null;
        
        // Act
        var userWithoutActiveLanguage = await _userRepository.UpdateUserAsync(updatedUser);
        var persistedUser = await _userRepository.GetUserAsync(user.UserId); 
        
        // Assert
        Assert.Null(userWithoutActiveLanguage.ActiveLanguage);
        Assert.Null(persistedUser.ActiveLanguage);
    }
}