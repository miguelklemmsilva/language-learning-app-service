using System.Diagnostics.CodeAnalysis;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using AWS.Services;
using Core.Helpers;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;
using Core.Services;

namespace Lambda.LambdaStartup;

[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Function))]
[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(APIGatewayHttpApiV2ProxyRequest))]
[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(APIGatewayHttpApiV2ProxyResponse))]
public class Function(
    IUserService userService,
    IUserLanguageService userLanguageService,
    IVocabularyService vocabularyService)
{
    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/user")]
    public async Task<User> GetUser([FromHeader] string authorization)
    {
        var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

        return await userService.GetUserAsync(userId);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/user")]
    public async Task<User> UpdateUser([FromHeader] string authorization, [FromBody] UpdateUserRequest updateRequest)
    {
        var username = AuthHelper.ParseToken(authorization).CognitoUsername;

        var user = await userService.UpdateUserAsync(new User
            { UserId = username, ActiveLanguage = updateRequest.ActiveLanguage });

        return user;
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/language")]
    public async Task<UserLanguage> UpdateLanguage([FromHeader] string authorization,
        [FromBody] UserLanguageRequest updateRequest)
    {
        var username = AuthHelper.ParseToken(authorization).CognitoUsername;

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
        var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

        return await userLanguageService.GetUserLanguagesAsync(userId);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Delete, "/language")]
    public async Task<RemoveUserLanguageResponse> RemoveLanguage([FromHeader] string authorization,
        [FromBody] RemoveUserLanguageRequest removeRequest)
    {
        var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

        return await userLanguageService.RemoveUserLanguageAsync(userId, removeRequest.Language);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/vocabulary/{language}")]
    public async Task<IEnumerable<GetUserVocabularyResponse>> GetUserVocabulary([FromHeader] string authorization,
        string language)
    {
        var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

        return await vocabularyService.GetUserVocabularyAsync(userId, language);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/vocabulary")]
    public async Task<IEnumerable<string>> AddVocabulary([FromHeader] string authorization,
        [FromBody] AddVocabularyRequest addRequest)
    {
        var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

        return await vocabularyService.AddVocabularyAsync(userId, addRequest);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Delete, "/vocabulary")]
    public async Task<Dictionary<string, string>> RemoveVocabulary([FromHeader] string authorization,
        [FromBody] RemoveVocabularyRequest removeRequest)
    {
        var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

        await vocabularyService.RemoveVocabularyAsync(userId, removeRequest);

        return new Dictionary<string, string> { { "message", "Vocabulary removed successfully" } };
    }
}