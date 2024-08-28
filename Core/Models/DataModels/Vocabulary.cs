namespace Core.Models.DataModels;

public class Vocabulary
{
    public required string UserId { get; set; }
    public required string Language { get; set; }
    public required string Word { get; set; }
    public long LastPracticed { get; set; }
    public int BoxNumber { get; set; }
}