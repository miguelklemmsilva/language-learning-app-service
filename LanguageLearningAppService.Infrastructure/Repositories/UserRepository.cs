using Amazon.DynamoDBv2.DataModel;
using Core.Interfaces;
using Core.Models.DataModels;

namespace LanguageLearningAppService.Infrastructure.Repositories;

public class UserRepository(IDynamoDBContext context) : IUserRepository
{
    public async Task<User> CreateUserAsync(User user)
    {
        await context.SaveAsync(user);
        return user;
    }

    public async Task<User?> GetUserAsync(string userId)
    {
        return await context.LoadAsync<User?>(userId);
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        await context.SaveAsync(user);
        return user;
    }
}