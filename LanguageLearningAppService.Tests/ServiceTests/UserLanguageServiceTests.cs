using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models.DataModels;
using LanguageLearningAppService.Tests.TestBuilders;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LanguageLearningAppService.Tests.ServiceTests
{
    [Collection("DynamoDb Collection")]
    public class UserLanguageServiceTests(DynamoDbFixture.DynamoDbFixture fixture)
    {
        private readonly IUserLanguageService _userLanguageService =
            fixture.ServiceProvider.GetRequiredService<IUserLanguageService>();

        private readonly IUserRepository
            _userRepository = fixture.ServiceProvider.GetRequiredService<IUserRepository>();

        [Fact]
        public async Task UpdateUserLanguageAsync_Should_Set_ActiveLanguage_When_None_Is_Set()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var userId = user.UserId;
            await _userRepository.CreateUserAsync(user);

            var userLanguage = new UserLanguage
            {
                UserId = userId, Language = Language.Spanish, Country = "Spain", Listening = true, Speaking = true,
                Proficiency = Proficiency.Beginner,
                Translation = true
            };

            // Act
            var result = await _userLanguageService.UpdateUserLanguageAsync(userLanguage);

            // Assert
            Assert.Equal(Language.Spanish, result.Language);

            var updatedUser = await _userRepository.GetUserAsync(userId);
            Assert.Equal(Language.Spanish, updatedUser.ActiveLanguage);
        }

        [Fact]
        public async Task UpdateUserLanguageAsync_Should_Not_Change_ActiveLanguage_If_Already_Set()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var userId = user.UserId;
            await _userRepository.CreateUserAsync(user);

            // Attempt to add a new language.
            var userLanguage = new UserLanguage
            {
                UserId = userId, Language = Language.Spanish, Country = "Spain", Listening = true, Speaking = true,
                Proficiency = Proficiency.Beginner,
                Translation = true
            };

            // Act
            var result = await _userLanguageService.UpdateUserLanguageAsync(userLanguage);

            // Assert
            // The user language record reflects the new language, but the user's active language remains unchanged.
            Assert.Equal(Language.Spanish, result.Language);

            var updatedUser = await _userRepository.GetUserAsync(userId);
            Assert.Equal(Language.Spanish, updatedUser.ActiveLanguage);
        }

        [Fact]
        public async Task RemoveUserLanguageAsync_Should_Return_RemovedLanguage_If_Not_Active()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var userId = user.UserId;
            await _userRepository.CreateUserAsync(user);

            // Seed a record for the active language.
            var langFrench = new UserLanguage
            {
                UserId = userId, Language = Language.French, Country = "France", Listening = true, Speaking = true,
                Proficiency = Proficiency.Beginner,
                Translation = true
            };
            await _userLanguageService.UpdateUserLanguageAsync(langFrench);

            // Seed an additional language record.
            var langSpanish = new UserLanguage
            {
                UserId = userId, Language = Language.Spanish, Country = "Spain", Listening = true, Speaking = true,
                Proficiency = Proficiency.Beginner,
                Translation = true
            };
            await _userLanguageService.UpdateUserLanguageAsync(langSpanish);

            // Act: Remove Spanish which is not the active language.
            var removedLanguage = await _userLanguageService.RemoveUserLanguageAsync(userId, Language.Spanish);

            // Assert
            Assert.Equal(Language.Spanish, removedLanguage);

            var updatedUser = await _userRepository.GetUserAsync(userId);
            Assert.Equal(Language.French, updatedUser.ActiveLanguage);
        }

        [Fact]
        public async Task
            RemoveUserLanguageAsync_Should_Update_ActiveLanguage_When_Removing_Active_And_RemainingLanguagesExist()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var userId = user.UserId;
            await _userRepository.CreateUserAsync(user);

            // First update call sets active language to Spanish.
            var langSpanish = new UserLanguage
            {
                UserId = userId, Language = Language.Spanish, Country = "Spain", Listening = true, Speaking = true,
                Proficiency = Proficiency.Beginner,
                Translation = true
            };
            await _userLanguageService.UpdateUserLanguageAsync(langSpanish);

            // Add another language record.
            var langFrench = new UserLanguage
            {
                UserId = userId, Language = Language.French, Country = "France", Listening = true, Speaking = true,
                Proficiency = Proficiency.Beginner,
                Translation = true
            };
            await _userLanguageService.UpdateUserLanguageAsync(langFrench);

            // Verify that the active language remains Spanish.
            var currentUser = await _userRepository.GetUserAsync(userId);
            Assert.Equal(Language.Spanish, currentUser.ActiveLanguage);

            // Act: Remove the active language (Spanish).
            var newActiveLanguage = await _userLanguageService.RemoveUserLanguageAsync(userId, Language.Spanish);

            // Assert: Expect the active language to update to French (the next available language).
            Assert.Equal(Language.French, newActiveLanguage);

            var updatedUser = await _userRepository.GetUserAsync(userId);
            Assert.Equal(Language.French, updatedUser.ActiveLanguage);
        }

        [Fact]
        public async Task
            RemoveUserLanguageAsync_Should_Update_ActiveLanguage_To_Null_When_Removing_Active_And_NoRemainingLanguages()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var userId = user.UserId;
            await _userRepository.CreateUserAsync(user);

            // Set active language to Spanish.
            var langSpanish = new UserLanguage
            {
                UserId = userId, Language = Language.Spanish, Country = "Spain", Listening = true, Speaking = true,
                Proficiency = Proficiency.Beginner,
                Translation = true
            };
            await _userLanguageService.UpdateUserLanguageAsync(langSpanish);

            // Act: Remove the active language.
            var newActiveLanguage = await _userLanguageService.RemoveUserLanguageAsync(userId, Language.Spanish);

            // Assert: Expect active language to be null when no languages remain.
            Assert.Null(newActiveLanguage);

            var updatedUser = await _userRepository.GetUserAsync(userId);
            Assert.Null(updatedUser.ActiveLanguage);
        }

        [Fact]
        public async Task GetUserLanguageAsync_Should_Return_Correct_UserLanguage()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var userId = user.UserId;
            await _userRepository.CreateUserAsync(user);

            // Add a user language record.
            var langGerman = new UserLanguage
            {
                UserId = userId, Language = Language.German, Country = "Germany", Listening = true, Speaking = true,
                Proficiency = Proficiency.Beginner,
                Translation = true
            };
            await _userLanguageService.UpdateUserLanguageAsync(langGerman);

            // Act
            var retrieved = await _userLanguageService.GetUserLanguageAsync(userId, Language.German);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal(Language.German, retrieved.Language);
            Assert.Equal(userId, retrieved.UserId);
        }
    }
}