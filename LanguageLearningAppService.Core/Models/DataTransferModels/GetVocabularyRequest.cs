using Core.Models.DataModels;

namespace Core.Models.DataTransferModels;

public class GetVocabularyRequest
{
    public required Language Language { get; set; }
}