using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Core.Models.DataModels;

namespace LanguageLearningAppService.Infrastructure.DynamoDataModels;

[DynamoDBTable("users")]
public class UserTable
{
    [DynamoDBHashKey]
    public required string UserId { get; set; }
    [DynamoDBRangeKey]
    public required string Email { get; set; }
    [DynamoDBProperty]
    public required Language? ActiveLanguage { get; set; }
}