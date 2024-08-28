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
        var userToUpdate = await GetUserAsync(user.UserId);

        var item = new Dictionary<string, AttributeValue>()
        {
            {
                "UserId",
                new AttributeValue { S = userToUpdate.UserId }
            },
            {
                "Email",
                new AttributeValue { S = userToUpdate.Email }
            },
            {
                "ActiveLanguage",
                user.ActiveLanguage != null
                    ? new AttributeValue { S = user.ActiveLanguage }
                    : new AttributeValue { NULL = true }
            }
        };

        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = item
        };

        await client.PutItemAsync(request);

        return await GetUserAsync(user.UserId);
    }
}