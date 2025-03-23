using Core.Interfaces;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Services;

public class UserService(
    IUserRepository userRepository,
    IUserLanguageService userLanguageService,
    IVocabularyService vocabularyService) : IUserService
{
    public async Task<User> CreateUserAsync(User user)
    {
        return await userRepository.CreateUserAsync(user);
    }

    public async Task<UserResponse> GetUserAsync(string userId)
    {
        var user = await userRepository.GetUserAsync(userId);

        ArgumentNullException.ThrowIfNull(user);

        if (user.ActiveLanguage != null)
            return new UserResponse
            {
                User = user,
                UserLanguages = await userLanguageService.GetUserLanguagesAsync(userId),
                Vocabulary = await vocabularyService.GetVocabularyAsync(userId, user.ActiveLanguage)
            };
        
        return new UserResponse
        {
            User = user
        };
    }

    public async Task<UserResponse> UpdateUserAsync(User user)
    {
        await GetUserAsync(user.UserId);

        await userRepository.UpdateUserAsync(user);

        return await GetUserAsync(user.UserId);
    }
}