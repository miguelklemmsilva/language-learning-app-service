using System;

namespace Core.Models.DataModels
{
    public class Token
    {
        public required string Sub { get; init; }
        public required string Email { get; init; }
    }
}