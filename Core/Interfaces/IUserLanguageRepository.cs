using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IUserLanguageRepository
{
    Task<UserLanguage> CreateUserLanguageAsync(UserLanguage userLanguage);
}