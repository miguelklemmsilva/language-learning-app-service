using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Core;
using Core.Helpers;
using Core.Interfaces;
using Core.Models.DataModels;

namespace LanguageLearningAppService.Infrastructure.Repositories;

public class AllowedVocabularyRepository(IAmazonDynamoDB client) : IAllowedVocabularyRepository
{
    private static readonly string TableName = Table.AllowedVocabulary.GetTableName();

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

    public async Task<IEnumerable<AllowedVocabulary>> GetWordsByCategoryAsync(string language)
    {
        var request = new QueryRequest
        {
            TableName = TableName,
            IndexName = "CategoryIndex",
            KeyConditionExpression = "#Language = :language",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#Language", "Language" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":language", new AttributeValue { S = language } }
            }
        };

        var response = await client.QueryAsync(request);

        return response.Items.Select(item => new AllowedVocabulary
            { Language = item["Language"].S, Word = item["Word"].S, Category = item["Category"].S }).ToList();
    }
}