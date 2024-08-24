using Core.Models.DataModels;

namespace AWS.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(User user);
}