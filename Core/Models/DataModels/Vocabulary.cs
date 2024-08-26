namespace Core.Models.DataModels;

public class Vocabulary
{
    public required string UserId { get; set; }
    public required string LanguageWord { get; set; }
    public long LastPracticed { get; set; }
    public int BoxNumber { get; set; }
}