using Core.Models.DataModels;

namespace Core.Models.DataTransferModels;

public class UserResponse
{
    public required User User { get; set; }
    public IEnumerable<UserLanguage>? UserLanguages { get; set; }
    public IEnumerable<Word?>? Vocabulary { get; set; }
}