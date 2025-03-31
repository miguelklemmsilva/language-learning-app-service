using Core.Models.DataModels;

namespace Core.Models.DataTransferModels;

public class VerifySentenceRequest
{
    public required string Original { get; set; }
    public required string Translation { get; set; }
    public required Language Language { get; set; }
}