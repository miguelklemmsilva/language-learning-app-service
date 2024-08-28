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
    public async Task<APIGatewayHttpApiV2ProxyResponse> GetUser([FromHeader] string authorization)
    {
        try
        {
            var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

            var user = await userService.GetUserAsync(userId);

            return ResponseHelper.CreateSuccessResponse(user, typeof(User));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/user")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> UpdateUser([FromHeader] string authorization,
        [FromBody] UpdateUserRequest updateRequest)
    {
        try
        {
            var username = AuthHelper.ParseToken(authorization).CognitoUsername;

            var user = await userService.UpdateUserAsync(new User
                { UserId = username, ActiveLanguage = updateRequest.ActiveLanguage });

            return ResponseHelper.CreateSuccessResponse(user, typeof(User));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/language")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> UpdateLanguage([FromHeader] string authorization,
        [FromBody] UserLanguageRequest updateRequest)
    {
        try
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

            return ResponseHelper.CreateSuccessResponse(await userLanguageService.UpdateUserLanguageAsync(userLanguage),
                typeof(UserLanguage));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/languages")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> GetUserLanguages([FromHeader] string authorization)
    {
        try
        {
            var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

            var languages = await userLanguageService.GetUserLanguagesAsync(userId);

            return ResponseHelper.CreateSuccessResponse(languages, typeof(IEnumerable<UserLanguage>));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Delete, "/language}")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> RemoveLanguage([FromHeader] string authorization,
        [FromQuery] string language)
    {
        try
        {
            var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

            var newLanguage = await userLanguageService.RemoveUserLanguageAsync(userId, language);

            if (newLanguage == null)
                throw new Exception("Language not found");

            return ResponseHelper.CreateSuccessResponse(new RemoveUserLanguageResponse { ActiveLanguage = newLanguage },
                typeof(RemoveUserLanguageResponse));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/vocabulary")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> GetVocabulary([FromHeader] string authorization,
        [FromQuery] string language)
    {
        try
        {
            var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

            var vocabularies = await vocabularyService.GetUserVocabularyAsync(userId, language);

            return ResponseHelper.CreateSuccessResponse(vocabularies, typeof(IEnumerable<GetUserVocabularyResponse>));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/vocabulary")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> AddVocabulary([FromHeader] string authorization,
        [FromBody] AddVocabularyRequest addRequest)
    {
        try
        {
            var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

            var vocabularies = await vocabularyService.AddVocabularyAsync(userId, addRequest);

            return ResponseHelper.CreateSuccessResponse(vocabularies, typeof(IEnumerable<string>));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Delete, "/vocabulary")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> RemoveVocabulary([FromHeader] string authorization, [FromQuery] string languageWord)
    {
        try
        {
            var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

            await vocabularyService.RemoveVocabularyAsync(userId, languageWord);

            return ResponseHelper.CreateSuccessResponse(
                new Dictionary<string, string> { { "message", "Vocabulary removed successfully" } },
                typeof(Dictionary<string, string>));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }
}