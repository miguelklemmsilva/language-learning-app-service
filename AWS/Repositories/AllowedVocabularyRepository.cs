using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Core.Interfaces;

namespace AWS.Repositories;

public class AllowedVocabularyRepository(IAmazonDynamoDB client) : IAllowedVocabularyRepository
{
    private const string TableName = "allowed_vocabulary";

    public async Task<bool> IsVocabularyAllowedAsync(string language, string word)
    {
        var request = new GetItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Word", new AttributeValue { S = word } },
                { "Language", new AttributeValue { S = language } }
            },
            ProjectionExpression = "Word"
        };

        var response = await client.GetItemAsync(request);

        // If the item exists, the word is allowed
        return response.HttpStatusCode == HttpStatusCode.OK && response.Item.Count > 0;
    }
}