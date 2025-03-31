using Amazon.DynamoDBv2.DataModel;

namespace Core.Models.DataModels;

[DynamoDBTable("vocabulary")]
public class Vocabulary
{
    [DynamoDBHashKey] public required string UserId { get; set; }

    // This composite key is stored in DynamoDB as "Language#Word"
    [DynamoDBRangeKey("sk")]
    public string LanguageWord
    {
        get => $"{Language}#{Word}";
        set
        {
            if (string.IsNullOrEmpty(value))
                return;
            var parts = value.Split('#');
            if (parts.Length != 2)
                throw new ArgumentException("Invalid composite key format.");
            // Parse and assign
            if (!Enum.TryParse<Language>(parts[0], out var lang))
                throw new ArgumentException($"Invalid language in composite key: {parts[0]}");
            Language = lang;
            Word = parts[1];
        }
    }

    // These properties won't be directly stored because they are part of the composite key.
    [DynamoDBIgnore] public Language Language { get; set; }
    [DynamoDBIgnore] public required string Word { get; set; }
    [DynamoDBProperty("LastSeen")] public long LastPracticed { get; set; }
    [DynamoDBProperty] public string? Category { get; set; }
    [DynamoDBProperty] public int BoxNumber { get; set; }
}