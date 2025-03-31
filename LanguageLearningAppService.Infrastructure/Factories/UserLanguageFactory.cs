using Amazon.DynamoDBv2.Model;
using Core.Models.DataModels;
using LanguageLearningAppService.Infrastructure.DynamoDataModels;
using static System.Enum;

namespace LanguageLearningAppService.Infrastructure.Factories;

public static class UserLanguageFactory
{
    public static UserLanguage Build(Dictionary<string, AttributeValue> item)
    {
        return new UserLanguage
        {
            UserId = item.GetRequiredString("UserId"),
            Language = item.GetRequiredEnum<Language>("Language"),
            Country = item["Country"].S,
            Translation = (bool)item["Translation"].BOOL!,
            Listening = (bool)item["Listening"].BOOL!,
            Speaking = (bool)item["Speaking"].BOOL!
        };
    }
}