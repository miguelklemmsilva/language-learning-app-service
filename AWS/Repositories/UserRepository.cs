using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AWS.Factories;
using Core.Interfaces;
using Core.Models.DataModels;

namespace AWS.Repositories;

public class UserRepository(IAmazonDynamoDB client) : IUserRepository
{
    private const string TableName = "users";

    public async Task<User> CreateUserAsync(User user)
    {

        var item = new Dictionary<string, AttributeValue>()
        {
            {
                "UserId",
                new AttributeValue { S = user.UserId }
            },
            {
                "Email",
                new AttributeValue { S = user.Email }
            }
        };
        
        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = item
        };
        
        await client.PutItemAsync(request);
        
        return user;
    }

    public async Task<User> GetUserAsync(string userId)
    {
        var key = new Dictionary<string, AttributeValue>
        {
            { "UserId", new AttributeValue { S = userId } }
        };

        var request = new GetItemRequest
        {
            TableName = TableName,
            Key = key
        };

        var response = await client.GetItemAsync(request);

        return UserFactory.Build(response.Item);
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        var key = new Dictionary<string, AttributeValue>
        {
            { "UserId", new AttributeValue { S = user.UserId } }
        };

        var updateExpression = "SET ActiveLanguage = :activeLanguage";
        var expressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            { ":activeLanguage", new AttributeValue { S = user.ActiveLanguage } }
        };

        // If ActiveLanguage is null or empty, remove it instead of setting it
        if (string.IsNullOrEmpty(user.ActiveLanguage))
        {
            updateExpression = "REMOVE ActiveLanguage";
            expressionAttributeValues.Clear(); // We don't need any expression attribute values for REMOVE
        }

        var request = new UpdateItemRequest
        {
            TableName = TableName,
            Key = key,
            UpdateExpression = updateExpression,
            ExpressionAttributeValues = expressionAttributeValues,
            ReturnValues = ReturnValue.ALL_NEW
        };

        var response = await client.UpdateItemAsync(request);

        return UserFactory.Build(response.Attributes);
    }
}