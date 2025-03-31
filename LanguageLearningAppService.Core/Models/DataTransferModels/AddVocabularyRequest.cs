using Core.Models.DataModels;

namespace Core.Models.DataTransferModels;

public class AddVocabularyRequest
{
    public required Language Language { get; set; }
    public required IEnumerable<string> Vocabulary { get; set; }
}