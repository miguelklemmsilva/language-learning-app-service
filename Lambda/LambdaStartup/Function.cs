using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Serialization.SystemTextJson;
using AWS.Repositories;
using AWS.Services;
using Core.Helpers;
using Core.Interfaces;
using Core.Models.DataModels;
using Core.Models.DataTransferModels;
using Core.Services;

namespace Lambda.LambdaStartup;

[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Function))]
[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(APIGatewayHttpApiV2ProxyRequest))]
[method: DynamicDependency(DynamicallyAccessedMemberTypes.All,  typeof(APIGatewayHttpApiV2ProxyResponse))]
public class Function(IUserService userService)
{
    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/user")]
    public async Task<User> GetUser([FromHeader] string Authorization)
    {
        var userId = AuthHelper.ParseToken(Authorization).CognitoUsername;
        
        var user = await userService.GetUserAsync(userId);

        return user;
    }
    
    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Put, "/user")]
    public async Task<User> UpdateUser([FromHeader] string Authorization, [FromBody] UpdateUserRequest updateRequest)
    {
        var username = AuthHelper.ParseToken(Authorization).CognitoUsername;
        
        var user = await userService.UpdateUserAsync(new User
            { UserId = username, ActiveLanguage = updateRequest.ActiveLanguage });

        return user;
    }
}