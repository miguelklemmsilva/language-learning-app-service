using Core.Models.DataModels;

namespace AWS.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(User user);
    Task<User> GetUserAsync(string userId);
    Task<User> UpdateUserAsync(User user);
}