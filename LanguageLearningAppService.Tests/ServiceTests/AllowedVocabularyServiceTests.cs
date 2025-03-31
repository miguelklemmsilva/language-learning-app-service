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
        var language = Language.Spanish;
        var word = "hola";

        var isAllowed = await _allowedVocabularyService.IsVocabularyAllowedAsync(language, word);

        Assert.True(isAllowed);
    }
        
    [Fact]
    public async Task Can_Check_If_Word_Is_Not_Allowed()
    {
        var language = Language.Spanish;
        var word = "hello";

        var isAllowed = await _allowedVocabularyService.IsVocabularyAllowedAsync(language, word);

        Assert.False(isAllowed);
    }
        
    [Fact]
    public async Task Can_Get_Categories()
    {
        var language = Language.Spanish;

        var categories = await _allowedVocabularyService.GetWordsByCategoryAsync(language);

        Assert.NotEmpty(categories);
    }
        
    [Fact]
    public async Task IsVocabularyAllowed_Is_CaseInsensitive()
    {
        var language = Language.Spanish;
        var wordLower = "hola";
        var wordUpper = "HOLA";

        var isAllowedLower = await _allowedVocabularyService.IsVocabularyAllowedAsync(language, wordLower);
        var isAllowedUpper = await _allowedVocabularyService.IsVocabularyAllowedAsync(language, wordUpper);

        Assert.Equal(isAllowedLower, isAllowedUpper);
    }
        
    [Fact]
    public async Task IsVocabularyAllowed_Returns_False_For_Empty_Word()
    {
        var language = Language.Spanish;
        string emptyWord = "";

        var isAllowed = await _allowedVocabularyService.IsVocabularyAllowedAsync(language, emptyWord);

        Assert.False(isAllowed);
    }
        
    [Fact]
    public async Task GetWordsByCategory_Groups_Words_Correctly()
    {
        var language = Language.Spanish;

        var categories = (await _allowedVocabularyService.GetWordsByCategoryAsync(language)).ToList();

        // For each returned category, ensure that every word's Category property matches the category's name.
        foreach (var category in categories)
        {
            Assert.All(category.Words, word => Assert.Equal(category.Name, word.Category));
        }
    }
        
    [Fact]
    public async Task GetWordsByCategory_Returns_Empty_For_Unseeded_Language()
    {
        var language = Language.German;

        var categories = await _allowedVocabularyService.GetWordsByCategoryAsync(language);

        Assert.Empty(categories);
    }
}