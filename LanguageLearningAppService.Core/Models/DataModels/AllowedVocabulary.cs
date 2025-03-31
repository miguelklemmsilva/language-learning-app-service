using Amazon.DynamoDBv2.DataModel;
using Core.Helpers;

namespace Core.Models.DataModels;

[DynamoDBTable("allowed_vocabulary")]
public class AllowedVocabulary
{
    // We assume that "Word" is the hash key.
    [DynamoDBHashKey]
    public required string Word { get; set; }
    
    [DynamoDBRangeKey]
    [DynamoDBGlobalSecondaryIndexHashKey(IndexNames = ["CategoryIndex"], Converter = typeof(EnumConverter<Language>))]
    public required Language Language { get; set; }

    [DynamoDBGlobalSecondaryIndexRangeKey(IndexNames = ["CategoryIndex"])]
    public required string Category { get; set; }
}
