using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Core.Interfaces;
using Core.Models.DataModels;
using Infrastructure.Factories;

namespace Infrastructure.Repositories;

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

        var request = new UpdateItemRequest
        {
            TableName = TableName,
            Key = key,
            ReturnValues = ReturnValue.ALL_NEW
        };

        if (string.IsNullOrEmpty(user.ActiveLanguage))
        {
            request.UpdateExpression = "REMOVE ActiveLanguage";
        }
        else
        {
            request.UpdateExpression = "SET ActiveLanguage = :activeLanguage";
            request.ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":activeLanguage", new AttributeValue { S = user.ActiveLanguage } }
            };
        }

        var response = await client.UpdateItemAsync(request);

        return UserFactory.Build(response.Attributes);
    }
}