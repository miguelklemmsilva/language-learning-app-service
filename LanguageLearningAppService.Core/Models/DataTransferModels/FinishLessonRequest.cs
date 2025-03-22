using Core.Models.DataModels;

namespace Core.Models.DataTransferModels;

public class FinishLessonRequest
{
    public required IEnumerable<FinishLessonSentence> Sentences { get; set; }
}

public class FinishLessonSentence
{
    public required string Language { get; set; }
    public int Mistakes { get; set; }
    public required string Type { get; set; }
    public required string Word { get; set; }
}