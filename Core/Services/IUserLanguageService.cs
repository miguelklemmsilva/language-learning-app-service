using Core.Models.DataModels;

namespace Core.Services;

public interface IUserLanguageService
{
    Task<UserLanguage> UpdateUserLanguageAsync(UserLanguage userLanguage);
}