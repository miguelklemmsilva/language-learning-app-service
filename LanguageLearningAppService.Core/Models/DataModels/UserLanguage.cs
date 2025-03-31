using Amazon.DynamoDBv2.DataModel;
using Core.Helpers;

namespace Core.Models.DataModels;

[DynamoDBTable("user_languages")]
public class UserLanguage
{
    [DynamoDBHashKey]
    public required string UserId { get; set; }
    [DynamoDBRangeKey(typeof(EnumConverter<Language>))]
    public required Language Language { get; set; }
    [DynamoDBProperty]
    public required string Country { get; set; }
    [DynamoDBProperty]
    public required bool Translation { get; set; }
    [DynamoDBProperty]
    public required bool Listening { get; set; }
    [DynamoDBProperty]
    public required bool Speaking { get; set; }
}