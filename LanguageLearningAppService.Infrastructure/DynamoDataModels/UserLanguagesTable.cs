namespace LanguageLearningAppService.Infrastructure.DynamoDataModels;

using Amazon.DynamoDBv2.DataModel;
using Core.Models.DataModels;

[DynamoDBTable("UserLanguages")]
public class UserLanguagesTable
{
    [DynamoDBHashKey]
    public required string UserId { get; set; }

    [DynamoDBRangeKey(Converter = typeof(Converter<Language, string>))]
    public Language Language { get; set; }

    [DynamoDBProperty]
    public required string Country { get; set; }

    [DynamoDBProperty]
    public bool Translation { get; set; }

    [DynamoDBProperty]
    public bool Listening { get; set; }

    [DynamoDBProperty]
    public bool Speaking { get; set; }
}