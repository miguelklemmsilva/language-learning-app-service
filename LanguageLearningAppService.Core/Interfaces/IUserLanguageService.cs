using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IUserLanguageService
{
    Task<UserLanguage> GetUserLanguageAsync(string userId, string language);
    Task<UserLanguage> UpdateUserLanguageAsync(UserLanguage userLanguage);
    Task<IEnumerable<UserLanguage>> GetUserLanguagesAsync(string userId);
    Task<string?> RemoveUserLanguageAsync(string userId, string? language);
}