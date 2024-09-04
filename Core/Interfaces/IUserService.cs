using Core.Models.DataModels;
using Core.Models.DataTransferModels;

namespace Core.Interfaces;

public interface IUserService
{
    Task<User> CreateUserAsync(User user);
    Task<UserResponse> GetUserAsync(string userId);
    Task<UserResponse> UpdateUserAsync(User user);
}