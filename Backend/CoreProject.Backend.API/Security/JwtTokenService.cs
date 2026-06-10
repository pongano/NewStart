using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.Identity.Entities;
using Microsoft.IdentityModel.Tokens;

namespace CoreProject.Backend.API.Security;

public sealed class JwtTokenService : IJwtTokenService
{
    public const string PermissionClaimType = "permission";
    private readonly IConfiguration _configuration;
    private readonly TimeProvider _timeProvider;

    public JwtTokenService(IConfiguration configuration, TimeProvider timeProvider)
    {
        _configuration = configuration;
        _timeProvider = timeProvider;
    }

    public JwtTokenResult CreateAccessToken(UserAccount userAccount, IReadOnlyCollection<string> permissionCodes)
    {
        var issuer = GetRequiredSetting("Authentication:Jwt:Issuer");
        var audience = GetRequiredSetting("Authentication:Jwt:Audience");
        var signingKey = GetRequiredSetting("Authentication:Jwt:SigningKey");
        var expiresAtUtc = _timeProvider.GetUtcNow().UtcDateTime.AddMinutes(GetAccessTokenMinutes());

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userAccount.Id.ToString()),
            new(ClaimTypes.NameIdentifier, userAccount.Id.ToString()),
            new(ClaimTypes.Name, userAccount.UserName),
            new(ClaimTypes.Email, userAccount.Email),
            new("display_name", userAccount.DisplayName)
        };

        claims.AddRange(permissionCodes.Select(x => new Claim(PermissionClaimType, x)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: _timeProvider.GetUtcNow().UtcDateTime,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtTokenResult
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiresAtUtc
        };
    }

    private int GetAccessTokenMinutes()
    {
        var configured = _configuration.GetValue<int?>("Authentication:Jwt:AccessTokenMinutes");
        return configured is > 0 ? configured.Value : 60;
    }

    private string GetRequiredSetting(string key)
    {
        var value = _configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Configuration value '{key}' was not found.");
        }

        return value;
    }
}
