using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Core.Interfaces;
using Core.Models.DataModels;

namespace Infrastructure.Repositories;

public class AllowedVocabularyRepository(IAmazonDynamoDB client) : IAllowedVocabularyRepository
{
    private const string TableName = "allowed_vocabulary";
    private const string LanguageIndexName = "Language-index";

    public async Task<bool> IsVocabularyAllowedAsync(string language, string word)
    {
        var request = new GetItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Word", new AttributeValue { S = word } },
                { "Language", new AttributeValue { S = language } }
            }
        };

        var response = await client.GetItemAsync(request);

        return response.Item != null;
    }

    public async Task<IEnumerable<AllowedVocabulary>> GetAllowedVocabularyByLanguageAsync(string language)
    {
        var request = new QueryRequest
        {
            TableName = TableName,
            IndexName = LanguageIndexName,
            KeyConditionExpression = "Language = :language",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":language", new AttributeValue { S = language } }
            }
        };

        var response = await client.QueryAsync(request);

        return response.Items.Select(item => new AllowedVocabulary
        {
            Word = item["Word"].S,
            Language = item["Language"].S,
            Category = item["Category"].S
        });
    }
}