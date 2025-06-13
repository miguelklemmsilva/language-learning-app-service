using System.Text.Json.Serialization;

namespace Core.Models.DataModels;

[JsonConverter(typeof(JsonStringEnumConverter<Language>))]
public enum Proficiency
{
    Beginner,
    Intermediate,
    Advanced
}
