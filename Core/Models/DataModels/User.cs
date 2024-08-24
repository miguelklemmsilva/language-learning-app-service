namespace Core.Models.DataModels;

public class User
{
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public string? ActiveLanguage { get; set; }
}