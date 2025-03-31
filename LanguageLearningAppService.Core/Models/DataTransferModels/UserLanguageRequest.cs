using Core.Models.DataModels;

namespace Core.Models.DataTransferModels;

public class UserLanguageRequest
{
    public required Language Language { get; set; }
    public required string Country { get; set; }
    public required bool Translation { get; set; }
    public required bool Listening { get; set; }
    public required bool Speaking { get; set; }
}