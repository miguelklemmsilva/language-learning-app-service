using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Core;
using Core.Helpers;
using LanguageLearningAppService.Infrastructure;

namespace LanguageLearningAppService.Tests.DynamoDbFixture;

public class TableCreator(IAmazonDynamoDB client)
{
    public async Task CreateTablesAsync()
    {
        // list tables
        var response = await client.ListTablesAsync();
        var tables = response.TableNames;

        if (!tables.Contains(Table.Users.GetTableName()))
            await CreateUsersTableAsync();

        if (!tables.Contains(Table.UserLanguages.GetTableName()))
            await CreateUserLanguagesTableAsync();

        if (!tables.Contains(Table.Vocabulary.GetTableName()))
            await CreateVocabularyTableAsync();

        if (!tables.Contains(Table.AllowedVocabulary.GetTableName()))
            await CreateAllowedVocabularyTableAsync();
    }

    private async Task CreateUsersTableAsync()
    {
        var request = new CreateTableRequest
        {
            TableName = Table.Users.GetTableName(),
            AttributeDefinitions = [new AttributeDefinition("UserId", ScalarAttributeType.S)],
            KeySchema = [new KeySchemaElement("UserId", KeyType.HASH)],
            BillingMode = BillingMode.PAY_PER_REQUEST
        };

        await client.CreateTableAsync(request);
        Console.WriteLine("Created table 'users'");
    }

    private async Task CreateUserLanguagesTableAsync()
    {
        var request = new CreateTableRequest
        {
            TableName = Table.UserLanguages.GetTableName(),
            AttributeDefinitions =
            [
                new AttributeDefinition("UserId", ScalarAttributeType.S),
                new AttributeDefinition("Language", ScalarAttributeType.S)
            ],
            KeySchema =
            [
                new KeySchemaElement("UserId", KeyType.HASH),
                new KeySchemaElement("Language", KeyType.RANGE)
            ],
            BillingMode = BillingMode.PAY_PER_REQUEST
        };

        await client.CreateTableAsync(request);
        Console.WriteLine("Created table 'user_languages'");
    }

    private async Task CreateVocabularyTableAsync()
    {
        var request = new CreateTableRequest
        {
            TableName = Table.Vocabulary.GetTableName(),
            AttributeDefinitions =
            [
                new AttributeDefinition("UserId", ScalarAttributeType.S),
                new AttributeDefinition("sk", ScalarAttributeType.S)
            ],
            KeySchema =
            [
                new KeySchemaElement("UserId", KeyType.HASH),
                new KeySchemaElement("sk", KeyType.RANGE)
            ],
            BillingMode = BillingMode.PAY_PER_REQUEST
        };

        await client.CreateTableAsync(request);
        Console.WriteLine("Created table 'vocabulary'");
    }

    private async Task CreateAllowedVocabularyTableAsync()
    {
        var request = new CreateTableRequest
        {
            TableName = Table.AllowedVocabulary.GetTableName(),
            AttributeDefinitions =
            [
                new AttributeDefinition("Word", ScalarAttributeType.S),
                new AttributeDefinition("Language", ScalarAttributeType.S),
                new AttributeDefinition("Category", ScalarAttributeType.S)
            ],
            KeySchema =
            [
                new KeySchemaElement("Word", KeyType.HASH),
                new KeySchemaElement("Language", KeyType.RANGE)
            ],
            GlobalSecondaryIndexes =
            [
                new GlobalSecondaryIndex
                {
                    IndexName = "CategoryIndex",
                    KeySchema =
                    [
                        new KeySchemaElement("Language", KeyType.HASH),
                        new KeySchemaElement("Category", KeyType.RANGE)
                    ],
                    Projection = new Projection { ProjectionType = ProjectionType.ALL }
                }
            ],
            BillingMode = BillingMode.PAY_PER_REQUEST
        };

        await client.CreateTableAsync(request);
        
        // Populate with test data
        var putRequest = new PutItemRequest
        {
            TableName = Table.AllowedVocabulary.GetTableName(),
            Item = new Dictionary<string, AttributeValue>
            {
                { "Word", new AttributeValue { S = "hola" } },
                { "Language", new AttributeValue { S = "Spanish" } },
                { "Category", new AttributeValue { S = "Greetings" } }
            }
        };
        
        await client.PutItemAsync(putRequest);
        
        Console.WriteLine("Created table 'allowed_vocabulary'");
    }
}