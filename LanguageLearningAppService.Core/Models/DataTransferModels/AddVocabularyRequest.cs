namespace Core.Models.DataTransferModels;

public class AddVocabularyRequest
{
    public required string Language { get; set; }
    public required IEnumerable<string> Vocabulary { get; set; }
}