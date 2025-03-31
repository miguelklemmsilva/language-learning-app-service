using Core.Interfaces;
using Core.Models.DataModels;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageLearningAppService.Tests.ServiceTests;

[Collection("DynamoDb Collection")]
public class AllowedVocabularyServiceTests(DynamoDbFixture.DynamoDbFixture fixture)
{
    private readonly IAllowedVocabularyService _allowedVocabularyService = fixture.ServiceProvider.GetRequiredService<IAllowedVocabularyService>();

    [Fact]
    public async Task Can_Check_If_Word_Is_Allowed()
    {
        // Arrange
        var language = Language.Spanish;
        var word = "hola";

        // Act
        var isAllowed = await _allowedVocabularyService.IsVocabularyAllowedAsync(language, word);

        // Assert
        Assert.True(isAllowed);
    }
    
    [Fact]
    public async Task Can_Check_If_Word_Is_Not_Allowed()
    {
        // Arrange
        var language = Language.Spanish;
        var word = "hello";

        // Act
        var isAllowed = await _allowedVocabularyService.IsVocabularyAllowedAsync(language, word);

        // Assert
        Assert.False(isAllowed);
    }
    
    [Fact]
    public async Task Can_Get_Categories()
    {
        // Arrange
        var language = Language.Spanish;

        // Act
        var categories = await _allowedVocabularyService.GetWordsByCategoryAsync(language);

        // Assert
        Assert.NotEmpty(categories);
    }
}