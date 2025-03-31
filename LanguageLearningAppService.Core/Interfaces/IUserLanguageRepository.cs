using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IUserLanguageRepository
{
    Task<UserLanguage> GetUserLanguageAsync(string userId, Language? language);
    Task<UserLanguage> CreateUserLanguageAsync(UserLanguage userLanguage);
    Task<IEnumerable<UserLanguage>> GetUserLanguagesAsync(string userId);
    Task RemoveUserLanguageAsync(string userId, Language? language);
}