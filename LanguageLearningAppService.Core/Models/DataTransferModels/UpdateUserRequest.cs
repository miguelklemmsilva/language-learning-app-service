using Core.Models.DataModels;

namespace Core.Models.DataTransferModels;

public class UpdateUserRequest
{
    public required Language ActiveLanguage { get; set; }
}