namespace Core.Models.DataModels;

public class Category
{
    public required string Name { get; set; }
    public required IEnumerable<Word> Words { get; set; }
}
