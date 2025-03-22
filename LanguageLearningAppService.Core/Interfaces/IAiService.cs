using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Interfaces;

public interface IAiService
{
    Task<VerifySentenceResponse> VerifySentenceAsync(VerifySentenceRequest request);
    Task<SentencesResponse> GenerateSentencesAsync(string userId);
}