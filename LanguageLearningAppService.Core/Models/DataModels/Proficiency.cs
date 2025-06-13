using System.Text.Json.Serialization;

namespace Core.Models.DataModels;

[JsonConverter(typeof(JsonStringEnumConverter<Proficiency>))]
public enum Proficiency
{
    Beginner,
    Intermediate,
    Advanced
}
