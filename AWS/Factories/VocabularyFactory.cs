using Amazon.DynamoDBv2.Model;
using Core.Models.DataModels;

namespace AWS.Factories;

public static class VocabularyFactory
{
    public static Vocabulary Build(Dictionary<string, AttributeValue> item)
    {
        var languageWord = item["sk"].S;
        
        // parse langauge and word separately Language#Word
        var language = languageWord.Split("#")[0];
        var word = languageWord.Split("#")[1];
        
        
        return new Vocabulary
        {
            UserId = item["UserId"].S,
            Language = language,
            Word = word,
            LastPracticed = long.Parse(item["LastSeen"].N),
            BoxNumber = int.Parse(item["BoxNumber"].N)
        };
    }
}