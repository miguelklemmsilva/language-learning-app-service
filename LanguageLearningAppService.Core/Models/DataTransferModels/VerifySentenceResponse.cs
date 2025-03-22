namespace Core.Models.DataTransferModels;

public class VerifySentenceResponse
{
    public bool IsCorrect { get; set; }
    public string? Explanation { get; set; }
}