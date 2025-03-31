using System.Reflection;
using System.Text.Json.Serialization;

namespace Core.Models.DataModels;

public enum Language
{
    [LanguageCode("es")] Spanish,
    [LanguageCode("de")] German,
    [LanguageCode("pt")] Portuguese,
    [LanguageCode("fr")] French,
    [LanguageCode("it")] Italian,
    [LanguageCode("jp")] Japanese
}

[JsonConverter(typeof(JsonStringEnumConverter<Language>))]
[AttributeUsage(AttributeTargets.Field)]
public sealed class LanguageCode(string? code) : Attribute
{
    private string? Code { get; } = code;

    public static string? GetLanguageCode(Language language)
    {
        // Get the type of the enum
        var type = language.GetType();
        // Retrieve the member information for the enum value
        var memInfo = type.GetMember(language.ToString());

        if (memInfo.Length > 0)
        {
            // Get the custom attribute applied to the member, if any
            var attributes = memInfo[0].GetCustomAttributes(typeof(LanguageCode), false);
            if (attributes.Length > 0)
            {
                return ((LanguageCode)attributes[0]).Code;
            }
        }

        return null;
    }
}