using System.Reflection;
using Core.Interfaces;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageLearningAppService.Tests.ServiceTests
{
    [Collection("DynamoDb Collection")]
    public class VocabularyServiceTests(DynamoDbFixture.DynamoDbFixture fixture)
    {
        private readonly IVocabularyService _vocabularyService = fixture.ServiceProvider.GetRequiredService<IVocabularyService>();
        
        [Fact]
        public async Task Can_Add_And_Retrieve_Vocabulary()
        {
            // Arrange
            const string userId = "testUser_AddRetrieve";
            const Language language = Language.Spanish;
            var words = new List<string> { "Hola" };

            var addRequest = new AddVocabularyRequest
            {
                Language = language,
                Vocabulary = words
            };

            // Act
            var addedWords = await _vocabularyService.AddVocabularyAsync(userId, addRequest);
            var vocabulary = (await _vocabularyService.GetVocabularyAsync(userId, language)).ToList();

            // Assert
            Assert.NotEmpty(addedWords);
            foreach (var word in words)
            {
                Assert.Contains(vocabulary.ToList(), v => 
                    v.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
            }
        }

        [Fact]
        public async Task Does_Not_Add_Duplicate_Vocabulary()
        {
            // Arrange
            var userId = "testUser_Duplicate";
            var language = Language.Spanish;
            var word = "hola";

            var addRequest = new AddVocabularyRequest
            {
                Language = language,
                Vocabulary = new List<string> { word }
            };

            // Act
            // First addition
            await _vocabularyService.AddVocabularyAsync(userId, addRequest);
            // Attempt duplicate addition
            var duplicateResult = await _vocabularyService.AddVocabularyAsync(userId, addRequest);

            // Assert: duplicate word should not be added.
            Assert.Empty(duplicateResult);
        }

        [Fact]
        public async Task Can_Remove_Vocabulary()
        {
            // Arrange
            var userId = "testUser_Remove";
            var language = Language.Spanish;
            var word = "hola";

            var addRequest = new AddVocabularyRequest
            {
                Language = language,
                Vocabulary = new List<string> { word }
            };

            // Act
            // Add vocabulary and verify it exists
            await _vocabularyService.AddVocabularyAsync(userId, addRequest);
            var vocabularyBefore = await _vocabularyService.GetVocabularyAsync(userId, language);
            Assert.Contains(vocabularyBefore, v => v.Word.Equals(word, StringComparison.OrdinalIgnoreCase));

            // Remove the vocabulary and verify removal
            await _vocabularyService.RemoveVocabularyAsync(userId, language, word);
            var vocabularyAfter = await _vocabularyService.GetVocabularyAsync(userId, language);
            Assert.DoesNotContain(vocabularyAfter, v => v.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task RemoveVocabulary_Throws_Exception_If_Word_Not_Found()
        {
            // This test verifies that RemoveVocabularyAsync throws if the word does not exist.
            var userId = "testUser_RemoveNonExisting";
            var language = Language.Spanish;
            var nonExistingWord = "doesnt_exist";

            // Act / Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _vocabularyService.RemoveVocabularyAsync(userId, language, nonExistingWord);
            });
        }

        [Fact]
        public async Task Can_Get_Words_To_Study()
        {
            // Arrange
            var userId = "testUser_Study";
            var language = Language.Spanish;
            var words = new List<string> { "hola", "soy", "eres" };

            var addRequest = new AddVocabularyRequest
            {
                Language = language,
                Vocabulary = words
            };

            // Act
            await _vocabularyService.AddVocabularyAsync(userId, addRequest);
            // Words in BoxNumber 1 are due immediately (MinutesUntilDue = 0)
            var wordsToStudy = await _vocabularyService.GetWordsToStudyAsync(userId, language, 10);

            // Assert
            var toStudy = wordsToStudy.ToList();
            Assert.NotEmpty(toStudy);
            foreach (var word in toStudy)
            {
                Assert.True(word.MinutesUntilDue <= 0);
            }
        }

        [Fact]
        public async Task Can_Finish_Lesson_And_Update_Vocabulary()
        {
            // Arrange
            var userId = "testUser_FinishLesson";
            var language = Language.Spanish;
            var word = "hola";

            var addRequest = new AddVocabularyRequest
            {
                Language = language,
                Vocabulary = new List<string> { word }
            };

            await _vocabularyService.AddVocabularyAsync(userId, addRequest);

            // Get initial vocabulary data.
            var vocabularyBefore = (await _vocabularyService.GetVocabularyAsync(userId, language))
                .FirstOrDefault(v => v.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(vocabularyBefore);
            var initialBoxNumber = vocabularyBefore.BoxNumber;

            // Build finish lesson request:
            // "translation" with 0 mistakes gives +3, "listening" with 1 mistake gives +1 => total +4
            var finishLessonRequest = new FinishLessonRequest
            {
                Sentences = new List<FinishLessonSentence>
                {
                    new()
                    {
                        Word = word,
                        Type = "translation",
                        Mistakes = 0,
                        Language = Language.Spanish
                    },
                    new() 
                    {
                        Word = word, 
                        Type = "listening", 
                        Mistakes = 1, 
                        Language = Language.Spanish
                    }
                }
            };

            // Act
            await _vocabularyService.FinishLessonAsync(userId, finishLessonRequest, language);
            var vocabularyAfter = (await _vocabularyService.GetVocabularyAsync(userId, language))
                .FirstOrDefault(v => v.Word.Equals(word, StringComparison.OrdinalIgnoreCase));

            // Calculate expected box number (initialBoxNumber + 4, capped between 1 and 9)
            var expectedBox = Math.Max(1, Math.Min(9, initialBoxNumber + 4));
            if (vocabularyAfter != null)
            {
                Assert.Equal(expectedBox, vocabularyAfter.BoxNumber);
                Assert.True(
                    vocabularyAfter.LastPracticed >= DateTimeOffset.UtcNow.AddSeconds(-10).ToUnixTimeSeconds(),
                    "LastPracticed should be updated to the current timestamp."
                );
            }
        }

        [Fact]
        public async Task FinishLesson_MultipleMistakes_Updates_BoxNumber_Correctly()
        {
            // This test checks coverage for negative scoring when mistakes are high.
            // translation with 3 mistakes => -1, listening with 4 mistakes => -2, etc.
            var userId = "testUser_FinishLessonMistakes";
            var language = Language.Spanish;
            var word = "hola";

            var addRequest = new AddVocabularyRequest
            {
                Language = language,
                Vocabulary = new List<string> { word }
            };

            await _vocabularyService.AddVocabularyAsync(userId, addRequest);

            var vocabularyBefore = (await _vocabularyService.GetVocabularyAsync(userId, language))
                .FirstOrDefault(v => v.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
            var initialBoxNumber = vocabularyBefore?.BoxNumber ?? 1;

            // translation(3 mistakes) => -1, translation(4 mistakes => default -2)
            // listening(0 mistakes) => +1
            // net total = -2
            var finishLessonRequest = new FinishLessonRequest
            {
                Sentences = new List<FinishLessonSentence>
                {
                    new() { Word = word, Type = "translation", Mistakes = 3, Language = language },
                    new() { Word = word, Type = "translation", Mistakes = 4, Language = language },
                    new() { Word = word, Type = "listening",  Mistakes = 0, Language = language }
                }
            };

            await _vocabularyService.FinishLessonAsync(userId, finishLessonRequest, language);

            var vocabularyAfter = (await _vocabularyService.GetVocabularyAsync(userId, language))
                .FirstOrDefault(v => v.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
            var expectedBox = Math.Max(1, Math.Min(9, initialBoxNumber - 2));
            if (vocabularyAfter != null) Assert.Equal(expectedBox, vocabularyAfter.BoxNumber);
        }
        
        [Fact]
        public void DecodeUnicodeEscapes_Returns_Correct_Decoded_String()
        {
            // Arrange
            var input = "Hello \\u0041\\u0042"; // Expected output: "Hello AB"

            // Use reflection to invoke the private static method DecodeUnicodeEscapes.
            var method = typeof(VocabularyService)
                .GetMethod("DecodeUnicodeEscapes", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.NotNull(method);

            var result = (string)method.Invoke(null, [input])!;

            // Assert
            Assert.Equal("Hello AB", result);
        }

        [Theory]
        [InlineData("\\u003F", "?")]
        [InlineData("\\u00A9", "©")]
        [InlineData("NoEscape", "NoEscape")]
        [InlineData("\\u0048\\u0069", "Hi")]
        public void DecodeUnicodeEscapes_VariousCases(string input, string expected)
        {
            // Test multiple inputs to ensure coverage
            var method = typeof(VocabularyService)
                .GetMethod("DecodeUnicodeEscapes", BindingFlags.NonPublic | BindingFlags.Static);

            var result = (string)method?.Invoke(null, [input])!;
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1,  0)]       // box 1 => 0 minutes until due
        [InlineData(2,  10)]      // box 2 => 10 minutes
        [InlineData(3,  60)]      // box 3 => 60 minutes
        [InlineData(4,  1440)]    // box 4 => 1440 minutes = 1 day
        [InlineData(5,  4320)]    // box 5 => 3 days
        [InlineData(6,  10080)]   // box 6 => 7 days
        [InlineData(7,  20160)]   // box 7 => 14 days
        [InlineData(8,  40320)]   // box 8 => 28 days
        [InlineData(9,  80640)]   // box 9 => 56 days
        public async Task GetVocabularyAsync_Assigns_Correct_MinutesUntilDue(int boxNumber, long expectedMinutes)
        {
            // This test checks that the service calculates the correct "MinutesUntilDue"
            // based on boxNumber and lastPracticed time.
            var userId = "testUser_BoxTime";
            var language = Language.Spanish;
            var word = $"test_box_{boxNumber}";

            // We'll add a vocab item with the given BoxNumber and lastPracticed in the past.
            // If lastPracticed = now minus X minutes => "GetVocabularyAsync" must compute the difference
            // for "MinutesUntilDue".
            var now = DateTimeOffset.UtcNow;
            // We'll place lastPracticed "0" minutes ago (basically now).
            var lastPracticed = now.ToUnixTimeSeconds();

            // Insert (or update) the vocab record in your real repository if doing an integration test:
            await _vocabularyService.UpdateVocabularyAsync(new Vocabulary
            {
                UserId = userId,
                Language = language,
                Word = word,
                BoxNumber = boxNumber,
                LastPracticed = lastPracticed,
            });

            // Act
            var vocab = await _vocabularyService.GetVocabularyAsync(userId, language);
            var target = vocab.FirstOrDefault(v => v.Word == word);

            // Because we used "lastPracticed = now", the difference in minutes is ~0,
            // so we expect the *full* box interval to remain.
            Assert.NotNull(target);
            Assert.Equal(expectedMinutes, target.MinutesUntilDue);
        }
    }
}
