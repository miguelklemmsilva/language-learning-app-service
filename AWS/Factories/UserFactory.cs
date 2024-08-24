using Amazon.DynamoDBv2.Model;
using Core.Models.DataModels;

namespace AWS.Factories;

public class UserFactory
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