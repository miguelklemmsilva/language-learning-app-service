namespace Core.Models.DataModels;

public class Category
{
    public required string Name { get; set; }
    public required IEnumerable<AllowedVocabulary> Words { get; set; }
}
