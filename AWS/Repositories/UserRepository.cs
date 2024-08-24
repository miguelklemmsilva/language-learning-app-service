using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AWS.Factories;
using Core.Interfaces;
using Core.Models.DataModels;

namespace AWS.Repositories;

public class UserRepository(IAmazonDynamoDB client) : IUserRepository
{
    private readonly string TABLE_NAME = "users";

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
            TableName = TABLE_NAME,
            Item = item
        };
        
        await client.PutItemAsync(request);
        
        return await GetUserAsync(user.UserId);
    }

    public async Task<User> GetUserAsync(string userId)
    {
        var key = new Dictionary<string, AttributeValue>
        {
            { "UserId", new AttributeValue { S = userId } }
        };

        var request = new GetItemRequest
        {
            TableName = TABLE_NAME,
            Key = key
        };

        var response = await client.GetItemAsync(request);

        return UserFactory.Build(response.Item);
    }
}