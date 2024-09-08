namespace Core.Models.DataModels;

public class CategoryGroup
{
    public string? Category { get; set; }
    public required List<Vocabulary> Words { get; set; }
}