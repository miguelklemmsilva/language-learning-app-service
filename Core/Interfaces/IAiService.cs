using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Interfaces;

public interface IAiService
{
    Task<SentencesResponse> GenerateSentencesAsync(string userId);
}