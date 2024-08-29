using Amazon.DynamoDBv2.Model;
using Core.Models.DataModels;

namespace Infrastructure.Factories;

public static class UserLanguageFactory
{
    public static UserLanguage Build(Dictionary<string, AttributeValue> item)
    {
        return new UserLanguage
        {
            UserId = item["UserId"].S,
            Language = item["Language"].S,
            Country = item["Country"].S,
            Translation = (bool)item["Translation"].BOOL!,
            Listening = (bool)item["Listening"].BOOL!,
            Speaking = (bool)item["Speaking"].BOOL!
        };
    }
}