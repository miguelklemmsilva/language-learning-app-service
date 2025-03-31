using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Core.Helpers;
using Core.Interfaces;
using Core.Models.DataModels;
using LanguageLearningAppService.Infrastructure.Factories;

namespace LanguageLearningAppService.Infrastructure.Repositories;

public class UserLanguageRepository(IAmazonDynamoDB client) : IUserLanguageRepository
{
    private static readonly string TableName = Table.UserLanguages.GetTableName();
    
    public async Task<UserLanguage> GetUserLanguageAsync(string userId, Language? language)
    {
        var key = new Dictionary<string, AttributeValue>
        {
            { "UserId", new AttributeValue { S = userId } },
            { "Language", new AttributeValue { S = language.ToString() } }
        };

        var request = new GetItemRequest
        {
            TableName = TableName,
            Key = key
        };

        var response = await client.GetItemAsync(request);

        return UserLanguageFactory.Build(response.Item);
    }
    
    public async Task<IEnumerable<UserLanguage>> GetUserLanguagesAsync(string userId)
    {
        var request = new QueryRequest
        {
            TableName = TableName,
            KeyConditionExpression = "UserId = :userId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":userId", new AttributeValue { S = userId } }
            }
        };

        var response = await client.QueryAsync(request);

        return response.Items.Select(UserLanguageFactory.Build);
    }

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
                new AttributeValue { S = userLanguage.Language.ToString() }
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
    
    public async Task RemoveUserLanguageAsync(string userId, Language? language)
    {
        var key = new Dictionary<string, AttributeValue>
        {
            { "UserId", new AttributeValue { S = userId } },
            { "Language", new AttributeValue { S = language.ToString() } }
        };

        var request = new DeleteItemRequest
        {
            TableName = TableName,
            Key = key
        };

        await client.DeleteItemAsync(request);
    }
}