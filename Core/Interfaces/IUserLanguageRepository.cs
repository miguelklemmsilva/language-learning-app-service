using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IUserLanguageRepository
{
    Task<UserLanguage> CreateUserLanguageAsync(UserLanguage userLanguage);
    Task<IEnumerable<UserLanguage>> GetUserLanguagesAsync(string userId);
    Task RemoveUserLanguageAsync(string userId, string language);
}