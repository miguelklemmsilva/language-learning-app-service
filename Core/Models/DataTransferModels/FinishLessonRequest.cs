using Core.Models.DataModels;

namespace Core.Models.DataTransferModels;

public class FinishLessonRequest
{
    public required IEnumerable<Sentence> Sentences { get; set; }
}