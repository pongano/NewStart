using System.Security.Cryptography;
using System.Text;
using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Infrastructure.Services;

public sealed class SecureRefreshTokenService : IRefreshTokenService
{
    public RefreshTokenValue CreateRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return new RefreshTokenValue
        {
            Token = token,
            TokenHash = HashToken(token)
        };
    }

    public string HashToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}
