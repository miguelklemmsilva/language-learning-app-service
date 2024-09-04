using Core.Models.DataModels;

namespace Core.Models.DataTransferModels;

public class SentencesResponse
{
    public required IEnumerable<Sentence> Sentences { get; set; }
    public required string IssueToken { get; set; }
}