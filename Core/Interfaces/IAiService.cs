using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IAiService
{
    Task<IEnumerable<Sentence>> GenerateSentencesAsync(string userId);
}