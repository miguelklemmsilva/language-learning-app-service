using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AWS.Factories;
using Core.Interfaces;
using Core.Models.DataModels;

namespace AWS.Repositories;

public class VocabularyRepository(IAmazonDynamoDB client) : IVocabularyRepository
{
    private const string TableName = "vocabulary";

    public async Task<Vocabulary> UpdateVocabularyAsync(Vocabulary vocabulary)
    {
        var item = new Dictionary<string, AttributeValue>
        {
            { "UserId", new AttributeValue { S = vocabulary.UserId } },
            { "Language#Word", new AttributeValue { S = vocabulary.LanguageWord } },
            { "LastSeen", new AttributeValue { N = vocabulary.LastPracticed.ToString() } },
            { "BoxNumber", new AttributeValue { N = vocabulary.BoxNumber.ToString() } }
        };

        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = item
        };

        await client.PutItemAsync(request);

        return await GetVocabularyAsync(vocabulary.UserId, vocabulary.LanguageWord);
    }

    public async Task<Vocabulary> GetVocabularyAsync(string userId, string languageWord)
    {
        var key = new Dictionary<string, AttributeValue>
        {
            { "UserId", new AttributeValue { S = userId } },
            { "Language#Word", new AttributeValue { S = languageWord } }
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
            KeyConditionExpression = "UserId = :userId AND begins_with(Language#Word, :language)",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":userId", new AttributeValue { S = userId } },
                { ":language", new AttributeValue { S = language } }
            }
        };

        var response = await client.QueryAsync(request);

        return response.Items.Select(VocabularyFactory.Build).ToList();
    }

    public async Task RemoveVocabularyAsync(Vocabulary vocabulary)
    {
        var request = new DeleteItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "UserId", new AttributeValue { S = vocabulary.UserId } },
                { "Language#Word", new AttributeValue { S = vocabulary.LanguageWord } }
            }
        };
        
        await client.DeleteItemAsync(request);
    }
}