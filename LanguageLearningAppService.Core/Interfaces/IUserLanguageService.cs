using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IUserLanguageService
{
    Task<UserLanguage> GetUserLanguageAsync(string userId, Language? language);
    Task<UserLanguage> UpdateUserLanguageAsync(UserLanguage userLanguage);
    Task<IEnumerable<UserLanguage>> GetUserLanguagesAsync(string userId);
    Task<Language?> RemoveUserLanguageAsync(string userId, Language language);
}