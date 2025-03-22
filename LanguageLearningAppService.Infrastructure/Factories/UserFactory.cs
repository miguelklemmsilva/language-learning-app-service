using Amazon.DynamoDBv2.Model;
using Core.Models.DataModels;

namespace LanguageLearningAppService.Infrastructure.Factories;

public static class UserFactory
{
    public static User Build(Dictionary<string, AttributeValue> item)
    {
        return new User
        {
            UserId = item["UserId"].S,
            Email = item["Email"].S,
            ActiveLanguage = item.TryGetValue("ActiveLanguage", out var activeLanguage) ? activeLanguage.S : null
        };
    }
    
}