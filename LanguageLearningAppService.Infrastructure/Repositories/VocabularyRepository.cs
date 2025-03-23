using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Core;
using Core.Helpers;
using Core.Interfaces;
using Core.Models.DataModels;
using LanguageLearningAppService.Infrastructure.Factories;

namespace LanguageLearningAppService.Infrastructure.Repositories;

public class VocabularyRepository(IAmazonDynamoDB client) : IVocabularyRepository
{
    private static readonly string TableName = Table.Vocabulary.GetTableName();

    public async Task<Vocabulary> UpdateVocabularyAsync(Vocabulary vocabulary)
    {
        var item = new Dictionary<string, AttributeValue>
        {
            { "UserId", new AttributeValue { S = vocabulary.UserId } },
            { "sk", new AttributeValue { S = $"{vocabulary.Language}#{vocabulary.Word}" } },
            { "LastSeen", new AttributeValue { N = vocabulary.LastPracticed.ToString() } },
            { "BoxNumber", new AttributeValue { N = vocabulary.BoxNumber.ToString() } }
        };

        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = item
        };

        await client.PutItemAsync(request);

        return await GetVocabularyAsync(vocabulary.UserId, vocabulary.Language, vocabulary.Word);
    }

    public async Task<Vocabulary> GetVocabularyAsync(string userId, string language, string word)
    {
        var key = new Dictionary<string, AttributeValue>
        {
            { "UserId", new AttributeValue { S = userId } },
            { "sk", new AttributeValue { S = $"{language}#{word}" } }
        };

        var request = new GetItemRequest
        {
            TableName = TableName,
            Key = key
        };

        var response = await client.GetItemAsync(request);

        return VocabularyFactory.Build(response.Item);
    }

    public async Task<IEnumerable<Vocabulary>> GetUserVocabularyAsync(string userId, string language)
    {
        var request = new QueryRequest
        {
            TableName = TableName,
            KeyConditionExpression = "UserId = :userId AND begins_with(sk, :language)",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":userId", new AttributeValue { S = userId } },
                { ":language", new AttributeValue { S = language } }
            }
        };

        var response = await client.QueryAsync(request);

        return response.Items.Select(VocabularyFactory.Build).ToList();
    }

    public async Task RemoveVocabularyAsync(string userId, string language, string word)
    {
        var request = new DeleteItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "UserId", new AttributeValue { S = userId } },
                { "sk", new AttributeValue { S = $"{language}#{word}" } }
            }
        };
        
        await client.DeleteItemAsync(request);
    }
}