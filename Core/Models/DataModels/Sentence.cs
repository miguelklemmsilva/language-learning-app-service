namespace Core.Models.DataModels;

public class Sentence
{
    public required string Original { get; set; }
    public required string Word { get; set; }
    public required string Type { get; set; }
    public string? Correct { get; set; }
    public int Mistakes { get; set; }
    public string? Translation { get; set; }
    public byte[]? Voice { get; set; }
    public required string Language { get; set; }
    public string? Alignment { get; set; }
}