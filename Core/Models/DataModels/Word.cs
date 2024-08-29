namespace Core.Models.DataModels;

public class Word : Vocabulary
{
    public long LastSeen { get; set; }
    public long MinutesUntilDue { get; set; }
}
