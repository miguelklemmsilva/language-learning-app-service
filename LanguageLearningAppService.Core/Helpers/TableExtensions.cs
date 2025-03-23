using System.Reflection;

namespace Core.Helpers;

[AttributeUsage(AttributeTargets.Field)]
public class TableNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}

public enum Table
{
    [TableName("users")]
    Users,
    
    [TableName("user_languages")]
    UserLanguages,
    
    [TableName("vocabulary")]
    Vocabulary,
    
    [TableName("allowed_vocabulary")]
    AllowedVocabulary
}

public static class TableExtensions
{
    public static string GetTableName(this Table table)
    {
        var type = table.GetType();
        var memberInfo = type.GetMember(table.ToString());
        var attribute = memberInfo[0].GetCustomAttribute<TableNameAttribute>();
        return attribute?.Name ?? table.ToString();
    }
}