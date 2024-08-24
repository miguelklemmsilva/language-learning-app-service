using Core.Models.DataModels;

namespace Core.Interfaces;

public interface IUserRepository
{
    Task<User> CreateUserAsync(User user);
    Task<User> GetUserAsync(string userId);
}