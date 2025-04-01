using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Core.Helpers;
using Core.Interfaces;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;
using static System.Enum;

namespace LanguageLearningAppService.Lambda;

[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Function))]
[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(APIGatewayHttpApiV2ProxyRequest))]
[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(APIGatewayHttpApiV2ProxyResponse))]
public class Function(
    IUserService userService,
    IUserLanguageService userLanguageService,
    IVocabularyService vocabularyService,
    IAllowedVocabularyService allowedVocabularyService,
    IAiService aiService,
    ITokenRepository tokenRepository,
    IChatGptService chatGptService)
{
    [LambdaFunction]
    public async Task<JsonObject> PreSignUpTrigger(JsonObject request)
    {
        var email = request["request"]!["userAttributes"]!["email"]!.ToString().ToLower();
        var sub = request["request"]!["userAttributes"]!["sub"]!.ToString();

        if (string.IsNullOrEmpty(email))
            throw new ArgumentException("Email not provided in the request.");

        var newUser = new User
        {
            UserId = sub,
            Email = email
        };

        await userService.CreateUserAsync(newUser);

        return request;
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/user")]
    public async Task<UserResponse> GetUser([FromHeader] string authorization)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        return await userService.GetUserAsync(userId);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/user")]
    public async Task<UserResponse> UpdateUser([FromHeader] string authorization,
        [FromBody] UpdateUserRequest updateRequest)
    {
        var username = AuthHelper.ParseToken(authorization).Sub;

        var user = await userService.UpdateUserAsync(new User
            { UserId = username, ActiveLanguage = updateRequest.ActiveLanguage });

        return user;
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/language")]
    public async Task<UserLanguage> UpdateLanguage([FromHeader] string authorization,
        [FromBody] UserLanguageRequest updateRequest)
    {
        var username = AuthHelper.ParseToken(authorization).Sub;

        var userLanguage = new UserLanguage
        {
            UserId = username,
            Language = updateRequest.Language,
            Country = updateRequest.Country,
            Translation = updateRequest.Translation,
            Listening = updateRequest.Listening,
            Speaking = updateRequest.Speaking
        };

        return await userLanguageService.UpdateUserLanguageAsync(userLanguage);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/languages")]
    public async Task<IEnumerable<UserLanguage>> GetUserLanguages([FromHeader] string authorization)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        var languages = await userLanguageService.GetUserLanguagesAsync(userId);

        return languages;
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Delete, "/language")]
    public async Task<RemoveUserLanguageResponse> RemoveLanguage([FromHeader] string authorization,
        [FromQuery] string language)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        var tryParse = TryParse<Language>(language, out var enumLanguage);

        if (!tryParse) throw new Exception($"{language} is not a valid language!");

        var newLanguage = await userLanguageService.RemoveUserLanguageAsync(userId, enumLanguage);

        return new RemoveUserLanguageResponse { ActiveLanguage = newLanguage };
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/vocabulary")]
    public async Task<IEnumerable<Word>> GetVocabulary([FromHeader] string authorization,
        [FromQuery] string language)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        var tryParse = TryParse<Language>(language, out var enumLanguage);

        if (!tryParse) throw new Exception($"{language} is not a valid language!");

        return await vocabularyService.GetVocabularyAsync(userId, enumLanguage);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/vocabulary")]
    public async Task<IEnumerable<string>> AddVocabulary([FromHeader] string authorization,
        [FromBody] AddVocabularyRequest addRequest)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        return await vocabularyService.AddVocabularyAsync(userId, addRequest);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Delete, "/vocabulary")]
    public async Task<Dictionary<string, string>> RemoveVocabulary([FromHeader] string authorization,
        [FromQuery] string language, [FromQuery] string word)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        var tryParse = TryParse<Language>(language, out var enumLanguage);

        if (!tryParse) throw new Exception($"{language} is not a valid language!");

        await vocabularyService.RemoveVocabularyAsync(userId, enumLanguage, word);

        return new Dictionary<string, string> { { "message", "Vocabulary removed successfully" } };
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/generatesentences")]
    public async Task<SentencesResponse> GenerateSentences([FromHeader] string authorization)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        return await aiService.GenerateSentencesAsync(userId);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Post, "/verifysentence")]
    public async Task<VerifySentenceResponse> VerifySentence([FromBody] VerifySentenceRequest verifyRequest)
    {
        return await chatGptService.VerifySentenceAsync(verifyRequest);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Post, "/finishlesson")]
    public async Task<Dictionary<string, string>> FinishLesson([FromHeader] string authorization,
        [FromBody] FinishLessonRequest finishRequest)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        var user = await userService.GetUserAsync(userId);

        var activeLanguage = user.User.ActiveLanguage ??
                             throw new ArgumentNullException(nameof(user.User.ActiveLanguage));

        await vocabularyService.FinishLessonAsync(userId, finishRequest, activeLanguage);

        return new Dictionary<string, string> { { "message", "Lesson finished successfully" } };
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Post, "/issuetoken")]
    public async Task<string> IssueToken()
    {
        var token = await tokenRepository.GetIssueTokenAsync();

        return token;
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/categories")]
    public async Task<IEnumerable<Category>> GetCategories([FromQuery] string language)
    {
        var tryParse = TryParse<Language>(language, out var enumLanguage);

        if (!tryParse) throw new Exception($"{language} is not a valid language!");

        var categories = await allowedVocabularyService.GetWordsByCategoryAsync(enumLanguage);

        return categories;
    }
}