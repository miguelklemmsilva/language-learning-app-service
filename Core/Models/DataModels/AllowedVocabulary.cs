namespace Core.Models.DataModels;

public class AllowedVocabulary
{
    public required string Word { get; set; }
    public required string Language { get; set; }
    public string? Category { get; set; }
}