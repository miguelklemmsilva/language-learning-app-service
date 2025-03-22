using Core.Interfaces;
using Core.Models.DataModels;

namespace Core.Services;

public class UserLanguageService(IUserLanguageRepository userLanguageRepository, IUserRepository userRepository) : IUserLanguageService
{
    public async Task<UserLanguage> UpdateUserLanguageAsync(UserLanguage userLanguage)
    {
        var user = await userRepository.GetUserAsync(userLanguage.UserId);
        
        // If the user doesn't already have an active language set, set it to the new language
        if (user.ActiveLanguage == null)
            await userRepository.UpdateUserAsync(new User { UserId = userLanguage.UserId, ActiveLanguage = userLanguage.Language });
        
        return await userLanguageRepository.CreateUserLanguageAsync(userLanguage);
    }

    public async Task<IEnumerable<UserLanguage>> GetUserLanguagesAsync(string userId)
    {
        var userLanguages = await userLanguageRepository.GetUserLanguagesAsync(userId);
        return userLanguages.Reverse();
    }

    public async Task<string?> RemoveUserLanguageAsync(string userId, string? language)
    {
        await userLanguageRepository.RemoveUserLanguageAsync(userId, language);

        var user = await userRepository.GetUserAsync(userId);

        if (language != user.ActiveLanguage) return language;
        
        var userLanguages = await GetUserLanguagesAsync(userId);
        var newActiveLanguage = userLanguages.FirstOrDefault()?.Language;
        await userRepository.UpdateUserAsync(new User { UserId = userId, ActiveLanguage = newActiveLanguage });
        return newActiveLanguage;
    }
    
    public async Task<UserLanguage> GetUserLanguageAsync(string userId, string language)
    {
        return await userLanguageRepository.GetUserLanguageAsync(userId, language);
    }
}