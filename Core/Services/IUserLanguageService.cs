using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Services;

public interface IUserLanguageService
{
    Task<UserLanguage> UpdateUserLanguageAsync(UserLanguage userLanguage);
    Task<IEnumerable<UserLanguage>> GetUserLanguagesAsync(string userId);
    Task<RemoveUserLanguageResponse> RemoveUserLanguageAsync(string userId, string language);
}