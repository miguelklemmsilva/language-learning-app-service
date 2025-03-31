using Amazon.DynamoDBv2.Model;

namespace LanguageLearningAppService.Infrastructure.DynamoDataModels;

public static class DynamoDbExtensions
{
    public static string GetRequiredString(this Dictionary<string, AttributeValue> item, string key)
    {
        if (!item.TryGetValue(key, out var attr) || string.IsNullOrWhiteSpace(attr.S))
            throw new ArgumentException($"Key '{key}' is missing or empty.");
        return attr.S;
    }

    public static T GetRequiredEnum<T>(this Dictionary<string, AttributeValue> item, string key) where T : struct, Enum
    {
        var str = item.GetRequiredString(key);
        if (!Enum.TryParse<T>(str, out var result))
            throw new ArgumentException($"Value '{str}' for key '{key}' is not a valid {typeof(T).Name}.");
        return result;
    }
}