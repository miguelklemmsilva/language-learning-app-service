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

        string updateExpression;
        var expressionAttributeValues = new Dictionary<string, AttributeValue>();
        var expressionAttributeNames = new Dictionary<string, string>();

        if (string.IsNullOrEmpty(user.ActiveLanguage))
        {
            // Check if ActiveLanguage attribute exists before attempting to remove it
            var getItemResponse = await client.GetItemAsync(new GetItemRequest
            {
                TableName = TableName,
                Key = key,
                ProjectionExpression = "ActiveLanguage"
            });

            if (getItemResponse.Item.ContainsKey("ActiveLanguage"))
            {
                updateExpression = "REMOVE ActiveLanguage";
            }
            else
            {
                // If ActiveLanguage doesn't exist, we need to update a different attribute
                // to avoid empty ExpressionAttributeValues
                updateExpression = "SET #updatedAt = :updatedAt";
                expressionAttributeNames.Add("#updatedAt", "UpdatedAt");
                expressionAttributeValues.Add(":updatedAt", new AttributeValue { S = DateTime.UtcNow.ToString("o") });
            }
        }
        else
        {
            updateExpression = "SET ActiveLanguage = :activeLanguage";
            expressionAttributeValues.Add(":activeLanguage", new AttributeValue { S = user.ActiveLanguage });
        }

        var request = new UpdateItemRequest
        {
            TableName = TableName,
            Key = key,
            UpdateExpression = updateExpression,
            ExpressionAttributeValues = expressionAttributeValues,
            ExpressionAttributeNames = expressionAttributeNames,
            ReturnValues = ReturnValue.ALL_NEW
        };

        var response = await client.UpdateItemAsync(request);

        return UserFactory.Build(response.Attributes);
    }}