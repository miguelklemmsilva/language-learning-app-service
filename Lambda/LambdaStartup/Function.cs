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
    public async Task<APIGatewayProxyResponse> GetUser([FromHeader] string authorization)
    {
        try
        {
            var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

            var user = await userService.GetUserAsync(userId);

            return ResponseHelper.CreateSuccessResponse(user);
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/user")]
    public async Task<APIGatewayProxyResponse> UpdateUser([FromHeader] string authorization, [FromBody] UpdateUserRequest updateRequest)
    {
        try
        {
            var username = AuthHelper.ParseToken(authorization).CognitoUsername;

            var user = await userService.UpdateUserAsync(new User
                { UserId = username, ActiveLanguage = updateRequest.ActiveLanguage });

            return ResponseHelper.CreateSuccessResponse(user);
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/language")]
    public async Task<APIGatewayProxyResponse> UpdateLanguage([FromHeader] string authorization,
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

            return ResponseHelper.CreateSuccessResponse(await userLanguageService.UpdateUserLanguageAsync(userLanguage));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/languages")]
    public async Task<APIGatewayProxyResponse> GetUserLanguages([FromHeader] string authorization)
    {
        try
        {
            var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

            return ResponseHelper.CreateSuccessResponse(await userLanguageService.GetUserLanguagesAsync(userId));
        } 
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Delete, "/language")]
    public async Task<APIGatewayProxyResponse> RemoveLanguage([FromHeader] string authorization,
        [FromBody] RemoveUserLanguageRequest removeRequest)
    {
        try
        {
            var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

            return ResponseHelper.CreateSuccessResponse(await userLanguageService.RemoveUserLanguageAsync(userId, removeRequest.Language));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/vocabulary/{language}")]
    public async Task<APIGatewayProxyResponse> GetUserVocabulary([FromHeader] string authorization,
        string language)
    {
        try
        {
            var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

            return ResponseHelper.CreateSuccessResponse(await vocabularyService.GetUserVocabularyAsync(userId, language));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/vocabulary")]
    public async Task<APIGatewayProxyResponse> AddVocabulary([FromHeader] string authorization,
        [FromBody] AddVocabularyRequest addRequest)
    {
        try {
            var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

            return ResponseHelper.CreateSuccessResponse(await vocabularyService.AddVocabularyAsync(userId, addRequest));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Delete, "/vocabulary")]
    public async Task<APIGatewayProxyResponse> RemoveVocabulary([FromHeader] string authorization,
        [FromBody] RemoveVocabularyRequest removeRequest)
    {
        try
        {
            var userId = AuthHelper.ParseToken(authorization).CognitoUsername;

            await vocabularyService.RemoveVocabularyAsync(userId, removeRequest);

            return ResponseHelper.CreateSuccessResponse(new { message = "Vocabulary removed" });
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }
}