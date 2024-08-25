using AWS.Services;
using Core.Interfaces;
using Core.Models.DataModels;

namespace Core.Services;

public class UserLanguageService(IUserLanguageRepository userLanguageRepository, IUserService userService) : IUserLanguageService
{
    
    public async Task<UserLanguage> UpdateUserLanguageAsync(UserLanguage userLanguage)
    {
        var user = await userService.GetUserAsync(userLanguage.UserId);
        
        // If the user doesn't already have an active language set, set it to the new language
        if (user.ActiveLanguage == null)
            await userService.UpdateUserAsync(new User { UserId = userLanguage.UserId, ActiveLanguage = userLanguage.Language });
        
        return await userLanguageRepository.CreateUserLanguageAsync(userLanguage);
    }

    public async Task<IEnumerable<UserLanguage>> GetUserLanguagesAsync(string userId)
    {
        return await userLanguageRepository.GetUserLanguagesAsync(userId);
    }

    public async Task RemoveUserLanguageAsync(string userId, string language)
    {
        await userLanguageRepository.RemoveUserLanguageAsync(userId, language);
    }
}