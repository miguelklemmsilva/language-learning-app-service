using Core.Models.DataModels;

namespace Core.Services;

public interface IUserLanguageService
{
    Task<UserLanguage> UpdateUserLanguageAsync(UserLanguage userLanguage);
    Task<IEnumerable<UserLanguage>> GetUserLanguagesAsync(string userId);
    Task RemoveUserLanguageAsync(string userId, string language);
}