using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Core.Models.DataModels;
using Infrastructure.Repositories;
using Xunit;

namespace Tests.Integration;

public class UserRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly IAmazonDynamoDB _client;
    private readonly UserRepository _userRepository;

    // We'll store or remove this user to keep tests consistent
    private const string TestUserId = "integration-test-user";

    public UserRepositoryIntegrationTests()
    {
        // For local DynamoDB, can do:
        // var config = new AmazonDynamoDBConfig { ServiceURL = "http://localhost:8000" };
        // _client = new AmazonDynamoDBClient("fakeKey", "fakeSecret", config);

        // testing against AWS:
        _client = new AmazonDynamoDBClient(RegionEndpoint.EUWest2);

        _userRepository = new UserRepository(_client);
    }

    [Fact]
    public async Task CreateUserAsync_StoresUserCorrectly()
    {
        // Arrange
        var user = new User
        {
            UserId = TestUserId,
            Email = "test_integration@example.com"
        };

        // Act
        var createdUser = await _userRepository.CreateUserAsync(user);

        // Assert
        Assert.NotNull(createdUser);
        Assert.Equal(TestUserId, createdUser.UserId);
        Assert.Equal("test_integration@example.com", createdUser.Email);

        // Now let's get it back from Dynamo to ensure it saved
        var retrievedUser = await _userRepository.GetUserAsync(TestUserId);
        Assert.NotNull(retrievedUser);
        Assert.Equal("test_integration@example.com", retrievedUser.Email);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldSetActiveLanguage()
    {
        // Arrange
        var user = new User
        {
            UserId = TestUserId,
            Email = "test_integration@example.com",
            ActiveLanguage = "Spanish"
        };

        // Act
        var updatedUser = await _userRepository.UpdateUserAsync(user);

        // Assert
        Assert.Equal("Spanish", updatedUser.ActiveLanguage);

        var retrievedUser = await _userRepository.GetUserAsync(TestUserId);
        Assert.Equal("Spanish", retrievedUser.ActiveLanguage);
    }

    #region Setup and Teardown

    // These hooks run before/after *each test* in XUnit when using IAsyncLifetime
    public async Task InitializeAsync()
    {
        // Optionally clear or reset the user item before tests
        try
        {
            await _client.DeleteItemAsync(new DeleteItemRequest
            {
                TableName = "users",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "UserId", new AttributeValue { S = TestUserId } }
                }
            });
        }
        catch
        {
            // Swallow any error if item doesn't exist
        }
    }

    public async Task DisposeAsync()
    {
        // Clean up the test user after each test
        try
        {
            await _client.DeleteItemAsync(new DeleteItemRequest
            {
                TableName = "users",
                Key = new System.Collections.Generic.Dictionary<string, AttributeValue>
                {
                    { "UserId", new AttributeValue { S = TestUserId } }
                }
            });
        }
        catch
        {
            // Swallow any error
        }
    }

    #endregion
}