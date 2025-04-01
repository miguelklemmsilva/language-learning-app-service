using Amazon.DynamoDBv2.DataModel;
using Core.Interfaces;
using Core.Models.DataModels;

namespace LanguageLearningAppService.Infrastructure.Repositories;

public class UserLanguageRepository(IDynamoDBContext context) : IUserLanguageRepository
{
    public async Task<UserLanguage> GetUserLanguageAsync(string userId, Language? language)
    {
        return await context.LoadAsync<UserLanguage>(userId, language.ToString());
    }

    public async Task<IEnumerable<UserLanguage>> GetUserLanguagesAsync(string userId)
    {
        var queryResult = context.QueryAsync<UserLanguage>(userId);
        return await queryResult.GetRemainingAsync();
    }

    public async Task<UserLanguage> CreateUserLanguageAsync(UserLanguage userLanguage)
    {
        await context.SaveAsync(userLanguage);
        return userLanguage;
    }

    public async Task RemoveUserLanguageAsync(string userId, Language? language)
    {
        await context.DeleteAsync<UserLanguage>(userId, language.ToString());
    }
}