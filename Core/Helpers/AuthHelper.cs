using System.IdentityModel.Tokens.Jwt;
using Core.Models.DataModels;

namespace Core.Helpers;

public static class AuthHelper
{
    public static Token ParseToken(IDictionary<string, string> headers)
    {
        var jwt = headers.TryGetValue("Authorization", out var header)
            ? header
            : string.Empty;

        if (string.IsNullOrEmpty(jwt))
        {
            throw new Exception("JWT token is missing");
        }

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        var tokenData = new Token
        {
            Sub = token.Claims.First(c => c.Type == "sub").Value,
            EmailVerified = bool.Parse(token.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value ?? "false"),
            Iss = token.Claims.First(c => c.Type == "iss").Value,
            CognitoUsername = token.Claims.First(c => c.Type == "cognito:username").Value,
            OriginJti = token.Claims.First(c => c.Type == "origin_jti").Value,
            Aud = token.Claims.First(c => c.Type == "aud").Value,
            EventId = token.Claims.First(c => c.Type == "event_id").Value,
            TokenUse = token.Claims.First(c => c.Type == "token_use").Value,
            AuthTime = long.Parse(token.Claims.FirstOrDefault(c => c.Type == "auth_time")?.Value ?? "0"),
            Exp = long.Parse(token.Claims.FirstOrDefault(c => c.Type == "exp")?.Value ?? "0"),
            Iat = long.Parse(token.Claims.FirstOrDefault(c => c.Type == "iat")?.Value ?? "0"),
            Jti = token.Claims.First(c => c.Type == "jti").Value,
            Email = token.Claims.First(c => c.Type == "email").Value
        };

        return tokenData;
    }
    
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
            EmailVerified = bool.Parse(token.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value ?? "false"),
            Iss = token.Claims.First(c => c.Type == "iss").Value,
            CognitoUsername = token.Claims.First(c => c.Type == "cognito:username").Value,
            OriginJti = token.Claims.First(c => c.Type == "origin_jti").Value,
            Aud = token.Claims.First(c => c.Type == "aud").Value,
            EventId = token.Claims.First(c => c.Type == "event_id").Value,
            TokenUse = token.Claims.First(c => c.Type == "token_use").Value,
            AuthTime = long.Parse(token.Claims.FirstOrDefault(c => c.Type == "auth_time")?.Value ?? "0"),
            Exp = long.Parse(token.Claims.FirstOrDefault(c => c.Type == "exp")?.Value ?? "0"),
            Iat = long.Parse(token.Claims.FirstOrDefault(c => c.Type == "iat")?.Value ?? "0"),
            Jti = token.Claims.First(c => c.Type == "jti").Value,
            Email = token.Claims.First(c => c.Type == "email").Value
        };

        return tokenData;
    }
}