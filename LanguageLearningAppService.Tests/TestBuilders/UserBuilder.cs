using Core.Models.DataModels;

namespace LanguageLearningAppService.Tests.TestBuilders;

public class UserBuilder
{
    private string _userId = Guid.NewGuid().ToString();
    private string _email = "test@example.com";
    private Language? _activeLanguage;
    
    public UserBuilder WithUserId(string userId)
    {
        _userId = userId;
        return this;
    }
    
    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }
    
    public UserBuilder WithActiveLanguage(Language activeLanguage)
    {
        _activeLanguage = activeLanguage;
        return this;
    }
    
    public User Build()
    {
        return new User
        {
            UserId = _userId,
            Email = _email,
            ActiveLanguage = _activeLanguage
        };
    }
}