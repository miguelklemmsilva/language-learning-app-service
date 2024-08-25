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
            TableName = TableName,
            Key = key
        };

        var response = await client.GetItemAsync(request);

        return UserFactory.Build(response.Item);
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        var updateExpressions = new List<string>();
        var expressionAttributeValues = new Dictionary<string, AttributeValue>();
        var expressionAttributeNames = new Dictionary<string, string>();

        if (user.Email != null)
        {
            updateExpressions.Add("#Email = :email");
            expressionAttributeValues[":email"] = new AttributeValue { S = user.Email };
            expressionAttributeNames["#Email"] = "Email";
        }

        if (user.ActiveLanguage != null)
        {
            updateExpressions.Add("#ActiveLanguage = :activeLanguage");
            expressionAttributeValues[":activeLanguage"] = new AttributeValue { S = user.ActiveLanguage };
            expressionAttributeNames["#ActiveLanguage"] = "ActiveLanguage";
        }

        if (updateExpressions.Count == 0)
        {
            // No fields to update
            return user;
        }

        var updateExpression = "SET " + string.Join(", ", updateExpressions);

        var request = new UpdateItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "UserId", new AttributeValue { S = user.UserId } }
            },
            UpdateExpression = updateExpression,
            ExpressionAttributeValues = expressionAttributeValues,
            ExpressionAttributeNames = expressionAttributeNames,
            ReturnValues = ReturnValue.ALL_NEW
        };

        var response = await client.UpdateItemAsync(request);
        return UserFactory.Build(response.Attributes);
    }
}