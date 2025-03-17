using Core.Interfaces;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;
using Core.Services;
using Moq;
using Xunit;

namespace Tests.Units;

public class VocabularyServiceTests
{
    [Fact]
    public async Task AddVocabularyAsync_ShouldAddWords_WhenTheyAreAllowedAndNotExisting()
    {
        // Arrange
        var mockRepository = new Mock<IVocabularyRepository>();
        var mockAllowedVocabService = new Mock<IAllowedVocabularyService>();

        // Make all words “allowed”
        mockAllowedVocabService
            .Setup(svc => svc.IsVocabularyAllowedAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Return an empty list so we pretend there's no existing vocabulary
        mockRepository
            .Setup(repo => repo.GetUserVocabularyAsync("testUser", "Spanish"))
            .ReturnsAsync(new List<Vocabulary>());

        // Construct the service with our mocks
        var service = new VocabularyService(mockRepository.Object, mockAllowedVocabService.Object);

        var request = new AddVocabularyRequest
        {
            Language = "Spanish",
            Vocabulary = ["hola", "adiós"]
        };

        // Act
        var addedWords = (await service.AddVocabularyAsync("testUser", request)).ToList();

        // Assert
        Assert.Equal(2, addedWords.Count());
        Assert.Contains("hola", addedWords);
        Assert.Contains("adiós", addedWords);

        // Ensure the repository was called for each word
        mockRepository.Verify(repo =>
            repo.UpdateVocabularyAsync(It.Is<Vocabulary>(v => v.Word == "hola")), Times.Once);
        mockRepository.Verify(repo =>
            repo.UpdateVocabularyAsync(It.Is<Vocabulary>(v => v.Word == "adiós")), Times.Once);
    }

    [Fact]
    public async Task AddVocabularyAsync_ShouldSkipWord_WhenVocabularyIsNotAllowed()
    {
        // Arrange
        var mockRepository = new Mock<IVocabularyRepository>();
        var mockAllowedVocabService = new Mock<IAllowedVocabularyService>();

        // Suppose "spam" is not allowed
        mockAllowedVocabService
            .Setup(svc => svc.IsVocabularyAllowedAsync("Spanish", "spam"))
            .ReturnsAsync(false);

        // The user has no existing vocabulary
        mockRepository
            .Setup(repo => repo.GetUserVocabularyAsync("testUser", "Spanish"))
            .ReturnsAsync(new List<Vocabulary>());

        var service = new VocabularyService(mockRepository.Object, mockAllowedVocabService.Object);

        var request = new AddVocabularyRequest
        {
            Language = "Spanish",
            Vocabulary = ["hola", "spam"]
        };

        // Act
        var addedWords = (await service.AddVocabularyAsync("testUser", request)).ToList();

        // Assert
        Assert.Single(addedWords);
        Assert.Contains("hola", addedWords);
        Assert.DoesNotContain("spam", addedWords);

        // Confirm "spam" was never updated in the repository
        mockRepository.Verify(repo =>
            repo.UpdateVocabularyAsync(It.Is<Vocabulary>(v => v.Word == "spam")), Times.Never);
    }

    [Fact]
    public async Task AddVocabularyAsync_ShouldSkipDuplicates()
    {
        // Arrange
        var mockRepository = new Mock<IVocabularyRepository>();
        var mockAllowedVocabService = new Mock<IAllowedVocabularyService>();

        // Mark all words as allowed
        mockAllowedVocabService
            .Setup(svc => svc.IsVocabularyAllowedAsync("Spanish", It.IsAny<string>()))
            .ReturnsAsync(true);

        // Fake that "hola" is already in the user’s vocab
        var existingVocab = new List<Vocabulary>
        {
            new Vocabulary
            {
                UserId = "testUser",
                Language = "Spanish",
                Word = "hola",
                LastPracticed = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                BoxNumber = 1
            }
        };

        mockRepository
            .Setup(repo => repo.GetUserVocabularyAsync("testUser", "Spanish"))
            .ReturnsAsync(existingVocab);

        var service = new VocabularyService(mockRepository.Object, mockAllowedVocabService.Object);

        var request = new AddVocabularyRequest
        {
            Language = "Spanish",
            Vocabulary = ["hola", "adiós"]
        };

        // Act
        var addedWords = (await service.AddVocabularyAsync("testUser", request)).ToList();

        // Assert
        // Should only add "adiós" because "hola" is duplicate
        Assert.Single(addedWords);
        Assert.Contains("adiós", addedWords);
        Assert.DoesNotContain("hola", addedWords);

        // Ensure only "adiós" was updated
        mockRepository.Verify(repo =>
            repo.UpdateVocabularyAsync(It.Is<Vocabulary>(v => v.Word == "hola")), Times.Never);
        mockRepository.Verify(repo =>
            repo.UpdateVocabularyAsync(It.Is<Vocabulary>(v => v.Word == "adiós")), Times.Once);
    }
}