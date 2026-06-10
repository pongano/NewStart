using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.Identity.Entities;
using System.Diagnostics.CodeAnalysis;

namespace CoreProject.Backend.Application.Identity.Auth.Login;

public sealed class LoginCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoginCommandHandler(
        IApplicationDbContext applicationDbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        IDateTimeProvider dateTimeProvider)
    {
        _applicationDbContext = applicationDbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<LoginResponse> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var identifier = command.Identifier.Trim();
        var password = command.Password;

        if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(password))
        {
            ThrowInvalidCredentials();
        }

        var users = await _applicationDbContext.ListUserAccountsAsync(cancellationToken);
        var user = users.FirstOrDefault(x =>
            string.Equals(x.UserName, identifier, StringComparison.OrdinalIgnoreCase)
            || string.Equals(x.Email, identifier, StringComparison.OrdinalIgnoreCase));

        if (user is null
            || !user.IsActive
            || string.IsNullOrWhiteSpace(user.PasswordHash)
            || !_passwordHasher.VerifyPassword(user.PasswordHash, password))
        {
            ThrowInvalidCredentials();
        }

        var permissions = await GetEffectivePermissionCodesAsync(user.Id, cancellationToken);
        var token = _jwtTokenService.CreateAccessToken(user, permissions);
        var refreshToken = _refreshTokenService.CreateRefreshToken();
        var refreshTokenExpiresAtUtc = _dateTimeProvider.UtcNow.AddDays(7);

        await _applicationDbContext.AddRefreshTokenAsync(new CoreProject.Backend.Domain.Identity.Entities.RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshToken.TokenHash,
            CreatedAtUtc = _dateTimeProvider.UtcNow,
            ExpiresAtUtc = refreshTokenExpiresAtUtc,
            CreatedByIp = command.IpAddress
        }, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            AccessToken = token.AccessToken,
            ExpiresAtUtc = token.ExpiresAtUtc,
            RefreshToken = refreshToken.Token,
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
    private static void ThrowInvalidCredentials()
    {
        throw new ValidationException(
            "Invalid credentials.",
            new Dictionary<string, string[]> { ["credentials"] = ["Invalid username/email or password."] });
    }
}
