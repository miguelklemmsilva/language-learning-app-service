using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Core.Interfaces;
using Core.Models.DataModels;

namespace AWS.Repositories;

public class UserLanguageRepository(IAmazonDynamoDB client) : IUserLanguageRepository
{
    private const string TableName = "user_languages";

    public async Task<UserLanguage> CreateUserLanguageAsync(UserLanguage userLanguage)
    {
        var item = new Dictionary<string, AttributeValue>()
        {
            {
                "UserId",
                new AttributeValue { S = userLanguage.UserId }
            },
            {
                "Language",
                new AttributeValue { S = userLanguage.Language }
            },
            {
                "Country",
                new AttributeValue { S = userLanguage.Country }
            },
            {
                "Translation",
                new AttributeValue { BOOL = userLanguage.Translation }
            },
            {
                "Listening",
                new AttributeValue { BOOL = userLanguage.Listening }
            },
            {
                "Speaking",
                new AttributeValue { BOOL = userLanguage.Speaking }
            }
        };
        
        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = item
        };
        
        await client.PutItemAsync(request);
        
        return userLanguage;
    }
}