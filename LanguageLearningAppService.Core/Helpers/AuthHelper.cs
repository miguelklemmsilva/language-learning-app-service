using System.IdentityModel.Tokens.Jwt;
using Core.Models.DataModels;

namespace Core.Helpers;

public static class AuthHelper
{
    public static Token ParseToken(string jwt)
    {
        if (string.IsNullOrEmpty(jwt))
        {
            throw new Exception("JWT token is missing");
        }

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        var tokenData = new Token
        {
            Sub = token.Claims.First(c => c.Type == "sub").Value,
            Email = token.Claims.First(c => c.Type == "sub").Value
        };

        return tokenData;
    }
}