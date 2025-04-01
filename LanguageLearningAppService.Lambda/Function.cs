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
    public async Task<APIGatewayHttpApiV2ProxyResponse> GetUser([FromHeader] string authorization)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        var user = await userService.GetUserAsync(userId);

        return ApiGatewayResponseManager.ToApiGatewaySuccessResponse(user);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/user")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> UpdateUser([FromHeader] string authorization,
        [FromBody] UpdateUserRequest updateRequest)
    {
        var username = AuthHelper.ParseToken(authorization).Sub;

        var user = await userService.UpdateUserAsync(new User
            { UserId = username, ActiveLanguage = updateRequest.ActiveLanguage });

        return ApiGatewayResponseManager.ToApiGatewaySuccessResponse(user);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/language")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> UpdateLanguage([FromHeader] string authorization,
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

        var userLanguageResponse = await userLanguageService.UpdateUserLanguageAsync(userLanguage);
        
        return ApiGatewayResponseManager.ToApiGatewaySuccessResponse(userLanguageResponse);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/languages")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> GetUserLanguages([FromHeader] string authorization)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        var languages = await userLanguageService.GetUserLanguagesAsync(userId);

        return ApiGatewayResponseManager.ToApiGatewaySuccessResponse(languages);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Delete, "/language")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> RemoveLanguage([FromHeader] string authorization,
        [FromQuery] string language)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        var tryParse = TryParse<Language>(language, out var enumLanguage);

        if (!tryParse) throw new Exception($"{language} is not a valid language!");

        var newLanguage = await userLanguageService.RemoveUserLanguageAsync(userId, enumLanguage);

        var removeUserLanguageResponse = new RemoveUserLanguageResponse { ActiveLanguage = newLanguage };
        
        return ApiGatewayResponseManager.ToApiGatewaySuccessResponse(removeUserLanguageResponse);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/vocabulary")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> GetVocabulary([FromHeader] string authorization,
        [FromQuery] string language)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        var tryParse = TryParse<Language>(language, out var enumLanguage);

        if (!tryParse) throw new Exception($"{language} is not a valid language!");

        var vocabulary = await vocabularyService.GetVocabularyAsync(userId, enumLanguage);
        
        return ApiGatewayResponseManager.ToApiGatewaySuccessResponse(vocabulary);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/vocabulary")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> AddVocabulary([FromHeader] string authorization,
        [FromBody] AddVocabularyRequest addRequest)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        var vocabulary = await vocabularyService.AddVocabularyAsync(userId, addRequest);
        
        return ApiGatewayResponseManager.ToApiGatewaySuccessResponse(vocabulary);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Delete, "/vocabulary")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> RemoveVocabulary([FromHeader] string authorization,
        [FromQuery] string language, [FromQuery] string word)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        var tryParse = TryParse<Language>(language, out var enumLanguage);

        if (!tryParse) throw new Exception($"{language} is not a valid language!");

        await vocabularyService.RemoveVocabularyAsync(userId, enumLanguage, word);

        var vocabularyRemovalResponse = new Dictionary<string, string> { { "message", "Vocabulary removed successfully" } };
        
        return ApiGatewayResponseManager.ToApiGatewaySuccessResponse(vocabularyRemovalResponse);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/generatesentences")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> GenerateSentences([FromHeader] string authorization)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        var sentences = await aiService.GenerateSentencesAsync(userId);
        
        return ApiGatewayResponseManager.ToApiGatewaySuccessResponse(sentences);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Post, "/verifysentence")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> VerifySentence([FromBody] VerifySentenceRequest verifyRequest)
    {
        var verifySentenceResponse = await chatGptService.VerifySentenceAsync(verifyRequest);
        
        return ApiGatewayResponseManager.ToApiGatewaySuccessResponse(verifySentenceResponse);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Post, "/finishlesson")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> FinishLesson([FromHeader] string authorization,
        [FromBody] FinishLessonRequest finishRequest)
    {
        var userId = AuthHelper.ParseToken(authorization).Sub;

        var user = await userService.GetUserAsync(userId);

        var activeLanguage = user.User.ActiveLanguage ??
                             throw new ArgumentNullException(nameof(user.User.ActiveLanguage));

        await vocabularyService.FinishLessonAsync(userId, finishRequest, activeLanguage);

        var successMessage = new Dictionary<string, string> { { "message", "Lesson finished successfully" } };
        
        return ApiGatewayResponseManager.ToApiGatewaySuccessResponse(successMessage);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Post, "/issuetoken")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> IssueToken()
    {
        var token = await tokenRepository.GetIssueTokenAsync();

        return ApiGatewayResponseManager.ToApiGatewaySuccessResponse(token);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/categories")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> GetCategories([FromQuery] string language)
    {
        var tryParse = TryParse<Language>(language, out var enumLanguage);

        if (!tryParse) throw new Exception($"{language} is not a valid language!");

        var categories = await allowedVocabularyService.GetWordsByCategoryAsync(enumLanguage);

        return ApiGatewayResponseManager.ToApiGatewaySuccessResponse(categories);
    }
}