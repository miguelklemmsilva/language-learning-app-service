using Core.Models.DataModels;

namespace Core.Models.DataTransferModels;

public class GetVocabularyResponse : Vocabulary
{
    public long LastSeen { get; set; }
    public long MinutesUntilDue { get; set; }
}
