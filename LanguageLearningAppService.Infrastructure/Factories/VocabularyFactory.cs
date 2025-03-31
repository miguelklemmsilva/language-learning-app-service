using Amazon.DynamoDBv2.Model;
using Core.Models.DataModels;

namespace LanguageLearningAppService.Infrastructure.Factories;

public static class VocabularyFactory
{
    public static Vocabulary Build(Dictionary<string, AttributeValue> item)
    {
        var languageWord = item["sk"].S;
        
        // parse langauge and word separately Language#Word
        var language = languageWord.Split("#")[0];
        var word = languageWord.Split("#")[1];
        
        var tryParse = Enum.TryParse<Language>(language, out var languageEnum);

        if (!tryParse)
            throw new Exception($"Invalid language value ${language} for User ${item["UserId"].S}.");
        
        
        return new Vocabulary
        {
            UserId = item["UserId"].S,
            Language = languageEnum,
            Word = word,
            LastPracticed = long.Parse(item["LastSeen"].N),
            BoxNumber = int.Parse(item["BoxNumber"].N)
        };
    }
}