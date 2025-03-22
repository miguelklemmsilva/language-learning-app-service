using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Core.Helpers;
using Core.Interfaces;
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
    IVocabularyService vocabularyService,
    IAllowedVocabularyService allowedVocabularyService,
    IAiService aiService,
    ITokenService tokenService)
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
        try
        {
            var userId = AuthHelper.ParseToken(authorization).Sub;

            var user = await userService.GetUserAsync(userId);

            return ResponseHelper.CreateSuccessResponse(user, typeof(UserResponse));
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
            var username = AuthHelper.ParseToken(authorization).Sub;

            var user = await userService.UpdateUserAsync(new User
                { UserId = username, ActiveLanguage = updateRequest.ActiveLanguage });

            return ResponseHelper.CreateSuccessResponse(user, typeof(UserResponse));
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
            var userId = AuthHelper.ParseToken(authorization).Sub;

            var languages = await userLanguageService.GetUserLanguagesAsync(userId);

            return ResponseHelper.CreateSuccessResponse(languages, typeof(IEnumerable<UserLanguage>));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Delete, "/language")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> RemoveLanguage([FromHeader] string authorization,
        [FromQuery] string language)
    {
        try
        {
            var userId = AuthHelper.ParseToken(authorization).Sub;

            var newLanguage = await userLanguageService.RemoveUserLanguageAsync(userId, language);

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
            var userId = AuthHelper.ParseToken(authorization).Sub;

            var vocabularies = await vocabularyService.GetVocabularyAsync(userId, language);

            return ResponseHelper.CreateSuccessResponse(vocabularies, typeof(IEnumerable<Word>));
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
            var userId = AuthHelper.ParseToken(authorization).Sub;

            foreach (var word in addRequest.Vocabulary)
            {
                Console.WriteLine($"Adding word: {word}");
            }

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
    public async Task<APIGatewayHttpApiV2ProxyResponse> RemoveVocabulary([FromHeader] string authorization,
        [FromQuery] string language, [FromQuery] string word)
    {
        try
        {
            var userId = AuthHelper.ParseToken(authorization).Sub;

            await vocabularyService.RemoveVocabularyAsync(userId, language, word);

            return ResponseHelper.CreateSuccessResponse(
                new Dictionary<string, string> { { "message", "Vocabulary removed successfully" } },
                typeof(Dictionary<string, string>));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/generatesentences")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> GenerateSentences([FromHeader] string authorization)
    {
        try
        {
            var userId = AuthHelper.ParseToken(authorization).Sub;

            var sentences = await aiService.GenerateSentencesAsync(userId);

            return ResponseHelper.CreateSuccessResponse(sentences, typeof(SentencesResponse));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Post, "/verifysentence")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> VerifySentence([FromBody] VerifySentenceRequest verifyRequest)
    {
        try
        {
            var result = await aiService.VerifySentenceAsync(verifyRequest);

            return ResponseHelper.CreateSuccessResponse(result, typeof(VerifySentenceResponse));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Post, "/finishlesson")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> FinishLesson([FromHeader] string authorization,
        [FromBody] FinishLessonRequest finishRequest)
    {
        try
        {
            var userId = AuthHelper.ParseToken(authorization).Sub;

            var user = await userService.GetUserAsync(userId);

            if (user.User.ActiveLanguage == null)
                throw new Exception("Active language not set");

            await vocabularyService.FinishLessonAsync(userId, finishRequest, user.User.ActiveLanguage);

            return ResponseHelper.CreateSuccessResponse(
                new Dictionary<string, string> { { "message", "Lesson finished successfully" } },
                typeof(Dictionary<string, string>));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Post, "/issuetoken")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> IssueToken()
    {
        try
        {
            var token = await tokenService.GetIssueTokenAsync();

            return ResponseHelper.CreateSuccessResponse(token, typeof(string));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/categories")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> GetCategories([FromQuery] string language)
    {
        try
        {
            var categories = await allowedVocabularyService.GetWordsByCategoryAsync(language);

            return ResponseHelper.CreateSuccessResponse(categories, typeof(IEnumerable<Category>));
        }
        catch (Exception e)
        {
            return ResponseHelper.CreateErrorResponse(e.Message);
        }
    }
}