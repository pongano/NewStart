using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Application.Identity.Auth.Login;
using System.Diagnostics.CodeAnalysis;

namespace CoreProject.Backend.Application.Identity.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RefreshTokenCommandHandler(
        IApplicationDbContext applicationDbContext,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        IDateTimeProvider dateTimeProvider)
    {
        _applicationDbContext = applicationDbContext;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<LoginResponse> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            ThrowInvalidRefreshToken();
        }

        var tokenHash = _refreshTokenService.HashToken(command.RefreshToken);
        var storedToken = await _applicationDbContext.FindRefreshTokenByHashAsync(tokenHash, cancellationToken);
        if (storedToken is null || storedToken.RevokedAtUtc is not null || storedToken.ExpiresAtUtc <= _dateTimeProvider.UtcNow)
        {
            ThrowInvalidRefreshToken();
        }

        var user = await _applicationDbContext.FindUserAccountByIdAsync(storedToken.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            ThrowInvalidRefreshToken();
        }

        storedToken.RevokedAtUtc = _dateTimeProvider.UtcNow;
        storedToken.RevokedByIp = command.IpAddress;

        var permissions = await GetEffectivePermissionCodesAsync(user.Id, cancellationToken);
        var accessToken = _jwtTokenService.CreateAccessToken(user, permissions);
        var newRefreshToken = _refreshTokenService.CreateRefreshToken();
        var refreshTokenExpiresAtUtc = _dateTimeProvider.UtcNow.AddDays(7);

        await _applicationDbContext.AddRefreshTokenAsync(new CoreProject.Backend.Domain.Identity.Entities.RefreshToken
        {
            UserId = user.Id,
            TokenHash = newRefreshToken.TokenHash,
            CreatedAtUtc = _dateTimeProvider.UtcNow,
            ExpiresAtUtc = refreshTokenExpiresAtUtc,
            CreatedByIp = command.IpAddress
        }, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            AccessToken = accessToken.AccessToken,
            ExpiresAtUtc = accessToken.ExpiresAtUtc,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc,
            UserId = user.Id,
            UserName = user.UserName,
            DisplayName = user.DisplayName,
            Permissions = permissions
        };
    }

    private async Task<IReadOnlyCollection<string>> GetEffectivePermissionCodesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var userRoles = await _applicationDbContext.ListUserRolesAsync(cancellationToken);
        var roleIds = userRoles
            .Where(x => x.UserId == userId)
            .Select(x => x.RoleId)
            .ToHashSet();

        var rolePermissions = await _applicationDbContext.ListRolePermissionsAsync(cancellationToken);
        var permissionIds = rolePermissions
            .Where(x => roleIds.Contains(x.RoleId))
            .Select(x => x.PermissionId)
            .ToHashSet();

        var permissions = await _applicationDbContext.ListPermissionsAsync(cancellationToken);

        return permissions
            .Where(x => permissionIds.Contains(x.Id))
            .Select(x => x.Code)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToList();
    }

    [DoesNotReturn]
    private static void ThrowInvalidRefreshToken()
    {
        throw new ValidationException(
            "Invalid refresh token.",
            new Dictionary<string, string[]> { ["refreshToken"] = ["Refresh token is invalid or expired."] });
    }
}
