using AWS.Services;
using Core.Interfaces;
using Core.Models.DataModels;

namespace Core.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<User> CreateUserAsync(User user)
    {
        var existing = await userRepository.GetUserAsync(user.UserId);
        
        if (existing != null)
        {
            throw new Exception("User already exists");
        }
        
        return await userRepository.CreateUserAsync(user);
    }
    
    public async Task<User> GetUserAsync(string userId)
    {
        var user = await userRepository.GetUserAsync(userId);
        
        if (user == null)
        {
            throw new Exception("User not found");
        }
        
        return user;
    }
    
    public async Task<User> UpdateUserAsync(User user)
    {
        var existingUser = await userRepository.GetUserAsync(user.UserId);
        
        if (existingUser == null)
        {
            throw new Exception("User not found");
        }
        
        return await userRepository.UpdateUserAsync(existingUser);
    }
}