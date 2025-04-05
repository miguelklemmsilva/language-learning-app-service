using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Core.Helpers;

namespace LanguageLearningAppService.Tests.DynamoDbFixture;

public class TableManager(IAmazonDynamoDB client)
{
    public async Task CreateTablesAsync()
    {
        var createTableTasks = new List<Task>
        {
            CreateUsersTableAsync(),
            CreateUserLanguagesTableAsync(),
            CreateVocabularyTableAsync(),
            CreateAllowedVocabularyTableAsync()
        };

        await Task.WhenAll(createTableTasks);
    }

    private async Task<bool> TableExistsAsync(string tableName)
    {
        try
        {
            var response = await client.DescribeTableAsync(tableName);
            return response.Table.TableStatus == TableStatus.ACTIVE;
        }
        catch (ResourceNotFoundException)
        {
            return false;
        }
    }

    private async Task CreateUsersTableAsync()
    {
        if (await TableExistsAsync(Table.Users.GetTableName()))
        {
            Console.WriteLine("Table 'users' already exists. Skipping creation.");
            return;
        }

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
        if (await TableExistsAsync(Table.UserLanguages.GetTableName()))
        {
            Console.WriteLine("Table 'user_languages' already exists. Skipping creation.");
            return;
        }

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
        if (await TableExistsAsync(Table.Vocabulary.GetTableName()))
        {
            Console.WriteLine("Table 'vocabulary' already exists. Skipping creation.");
            return;
        }

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
        if (await TableExistsAsync(Table.AllowedVocabulary.GetTableName()))
        {
            Console.WriteLine("Table 'allowed_vocabulary' already exists. Skipping creation.");
            return;
        }

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
        Console.WriteLine("Created table 'allowed_vocabulary'");

        await PopulateAllowedVocabularyTestDataAsync();
    }

    private async Task PopulateAllowedVocabularyTestDataAsync()
    {
        var testData = new List<Dictionary<string, AttributeValue>>
        {
            new()
            {
                { "Word", new AttributeValue { S = "hola" } },
                { "Language", new AttributeValue { S = "Spanish" } },
                { "Category", new AttributeValue { S = "Greetings" } }
            },
            new()
            {
                { "Word", new AttributeValue { S = "soy" } },
                { "Language", new AttributeValue { S = "Spanish" } },
                { "Category", new AttributeValue { S = "Verbs" } }
            },
            new()
            {
                { "Word", new AttributeValue { S = "eres" } },
                { "Language", new AttributeValue { S = "Spanish" } },
                { "Category", new AttributeValue { S = "Verbs" } }
            }
        };

        foreach (var item in testData)
        {
            var putRequest = new PutItemRequest
            {
                TableName = Table.AllowedVocabulary.GetTableName(),
                Item = item
            };

            await client.PutItemAsync(putRequest);
        }
    }

    public async Task DeleteTables()
    {
        var deleteTablesTask = new List<Task>
        {
            client.DeleteTableAsync(Table.Users.GetTableName()),
            client.DeleteTableAsync(Table.Vocabulary.GetTableName()),
            client.DeleteTableAsync(Table.UserLanguages.GetTableName()),
            client.DeleteTableAsync(Table.AllowedVocabulary.GetTableName())
        };

        await Task.WhenAll(deleteTablesTask);
    }
}