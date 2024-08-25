using System;

namespace Core.Models.DataModels
{
    public class Token
    {
        public string Sub { get; set; }
        public bool EmailVerified { get; set; }
        public string Iss { get; set; }
        public string CognitoUsername { get; set; }
        public string OriginJti { get; set; }
        public string Aud { get; set; }
        public string EventId { get; set; }
        public string TokenUse { get; set; }
        public long AuthTime { get; set; }
        public long Exp { get; set; }
        public long Iat { get; set; }
        public string Jti { get; set; }
        public string Email { get; set; }
    }
}