using Amazon.DynamoDBv2.DataModel;
using Core.Helpers;

namespace Core.Models.DataModels;

[DynamoDBTable("users")]
public class User
{
    [DynamoDBHashKey]
    public required string UserId { get; set; }
    [DynamoDBProperty]
    public string? Email { get; set; }
    [DynamoDBProperty(typeof(EnumConverter<Language>))]
    public Language? ActiveLanguage { get; set; }
}