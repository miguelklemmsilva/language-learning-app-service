using Amazon.DynamoDBv2.Model;
using Core.Models.DataModels;

namespace AWS.Factories;

public static class VocabularyFactory
{
    public static Vocabulary Build(Dictionary<string, AttributeValue> item)
    {
        return new Vocabulary
        {
            UserId = item["UserId"].S,
            LanguageWord = item["sk"].S,
            LastPracticed = long.Parse(item["LastSeen"].N),
            BoxNumber = int.Parse(item["BoxNumber"].N)
        };
    }
}