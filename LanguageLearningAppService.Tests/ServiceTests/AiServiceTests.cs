using Core.Interfaces;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;
using LanguageLearningAppService.Tests.TestBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageLearningAppService.Tests.ServiceTests
{
    [Collection("DynamoDb Collection")]
    public class AiServiceTests(DynamoDbFixture.DynamoDbFixture fixture)
    {
        private readonly IAiService _aiService = fixture.ServiceProvider.GetRequiredService<IAiService>();
        private readonly IChatGptService _chatGptService = fixture.ServiceProvider.GetRequiredService<IChatGptService>();
        private readonly ITranslationService _translationService = fixture.ServiceProvider.GetRequiredService<ITranslationService>();
        private readonly IUserService _userService = fixture.ServiceProvider.GetRequiredService<IUserService>();
        private readonly IUserLanguageService _userLanguageService = fixture.ServiceProvider.GetRequiredService<IUserLanguageService>();
        private readonly IVocabularyService _vocabularyService = fixture.ServiceProvider.GetRequiredService<IVocabularyService>();

        [Fact]
        public async Task GenerateSentencesAsync_Should_Return_Correct_SentencesResponse()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var userId = user.UserId;
            await _userService.CreateUserAsync(user);

            // Set active language via user language service with at least one active study type (Translation = true).
            var userLanguage = new UserLanguage
            {
                UserId = userId,
                Language = Language.Spanish,
                Translation = true,
                Listening = false,
                Speaking = false,
                Country = "Spain"
            };
            await _userLanguageService.UpdateUserLanguageAsync(userLanguage);

            // IMPORTANT:
            // Ensure that the vocabulary repository for Spanish is seeded with at least one word.
            // (If your implementation does not auto-seed vocabulary data, add vocabulary records prior to calling the AI service.)
            await _vocabularyService.AddVocabularyAsync(userId, new AddVocabularyRequest
            {
                Language = Language.Spanish,
                Vocabulary = ["hola"]
            });

            // Act
            var response = await _aiService.GenerateSentencesAsync(userId);

            // Assert
            Assert.NotNull(response);
            Assert.Equal("test-token", response.IssueToken);
            Assert.NotEmpty(response.Sentences);
            // Each sentence should follow the simulated responses:
            // - Original: "Sentence for " + <word>
            // - Translation: original sentence + " translated"
            // - Alignment as provided by the test translation repository.
            foreach (var sentence in response.Sentences)
            {
                Assert.StartsWith("Sentence for ", sentence.Original);
                Assert.EndsWith(" translated", sentence.Translation);
                Assert.Contains("dummy-alignment", sentence.Alignment);
                Assert.Equal(Language.Spanish, sentence.Language);
                Assert.Equal("Spain", sentence.Country);
            }
        }

        [Fact]
        public async Task GenerateSentencesAsync_Should_Throw_Exception_When_No_Active_Exercises()
        {
            // Arrange
            var userId = "ai-user2";
            // Create a user.
            var user = new User { UserId = userId, Email = "ai-user2@example.com", ActiveLanguage = null };
            await _userService.CreateUserAsync(user);

            // Set active language with no study types active.
            var userLanguage = new UserLanguage
            {
                UserId = userId,
                Language = Language.French,
                Translation = false,
                Listening = false,
                Speaking = false,
                Country = "France"
            };
            await _userLanguageService.UpdateUserLanguageAsync(userLanguage);

            // Act & Assert: Expect an exception because there are no active exercises.
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _aiService.GenerateSentencesAsync(userId);
            });
        }

        [Fact]
        public async Task ChatGptService_GenerateSentenceAsync_Should_Return_GeneratedSentence()
        {
            // Arrange
            var word = "testword";
            // Act
            var sentence = await _chatGptService.GenerateSentenceAsync(word, Language.Italian, "Italy");
            // Assert: Our TestChatGptRepository returns "Sentence for " + word.
            Assert.Equal("Sentence for " + word, sentence);
        }

        [Fact]
        public async Task ChatGptService_VerifySentenceAsync_Should_Return_Correct_Response()
        {
            // Arrange
            var request = new VerifySentenceRequest
            {
                Language = Language.German,
                Original = "Original sentence",
                Translation = "Translated sentence"
            };

            // Act
            var response = await _chatGptService.VerifySentenceAsync(request);

            // Assert
            Assert.True(response.IsCorrect);
            Assert.Equal("", response.Explanation);
        }

        [Fact]
        public async Task TranslationService_TranslateSentenceAsync_Should_Return_TranslatedTextItem()
        {
            // Arrange
            var sentence = "Sentence for test";
            // Act
            var translated = await _translationService.TranslateSentenceAsync(sentence, Language.Spanish);
            // Assert
            Assert.NotNull(translated);
            var translation = translated.Translations.First();
            Assert.Equal(sentence + " translated", translation.Text);
            Assert.Contains("dummy-alignment", translation.Alignment.Projections);
        }
    }
}