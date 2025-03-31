using Amazon.DynamoDBv2.Model;
using Core.Models.DataModels;
using LanguageLearningAppService.Infrastructure.DynamoDataModels;
using static System.Enum;

namespace LanguageLearningAppService.Infrastructure.Factories;

public static class UserFactory
{
    public static User Build(Dictionary<string, AttributeValue> item)
    {
        var user = new User
        {
            UserId = item.GetRequiredString("UserId"),
            Email = item.GetRequiredString("Email"),
        };

        if (item.TryGetValue("ActiveLanguage", out var activeLanguageObj) &&
            TryParse(activeLanguageObj.S, out Language activeLanguageEnum))
        {
            user.ActiveLanguage = activeLanguageEnum;
        }

        return user;
    }
}