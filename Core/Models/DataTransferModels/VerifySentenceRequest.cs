namespace Core.Models.DataTransferModels;

public class VerifySentenceRequest
{
    public required string Original { get; set; }
    public required string Translation { get; set; }
    public required string Language { get; set; }
}