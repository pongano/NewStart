using System.Net.Mail;
using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Application.Common.Security;
using CoreProject.Backend.Domain.AccessControl.Entities;
using CoreProject.Backend.Domain.Identity.Entities;

namespace CoreProject.Backend.Application.Identity.Auth.BootstrapAdmin;

public sealed class BootstrapAdminCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPasswordHasher _passwordHasher;

    public BootstrapAdminCommandHandler(
        IApplicationDbContext applicationDbContext,
        IDateTimeProvider dateTimeProvider,
        IPasswordHasher passwordHasher)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
        _passwordHasher = passwordHasher;
    }

    public async Task<BootstrapAdminResponse> HandleAsync(
        BootstrapAdminCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if ((await _applicationDbContext.ListUserAccountsAsync(cancellationToken)).Any())
        {
            throw new ValidationException(
                "Bootstrap admin is available only before the first user exists.",
                new Dictionary<string, string[]> { ["bootstrap"] = ["User accounts already exist."] });
        }

        var userName = command.UserName.Trim();
        var email = command.Email.Trim();
        var displayName = command.DisplayName.Trim();
        Validate(userName, email, displayName, command.Password);

        var now = _dateTimeProvider.UtcNow;
        var user = new UserAccount
        {
            UserName = userName,
            Email = email,
            DisplayName = displayName,
            PasswordHash = _passwordHasher.HashPassword(command.Password),
            IsActive = true,
            CreatedAtUtc = now,
            CreatedBy = "bootstrap"
        };

        var role = new Role
        {
            Code = "ADMIN",
            Name = "Administrator",
            Description = "Initial full-access administrator role.",
            IsActive = true,
            CreatedAtUtc = now,
            CreatedBy = "bootstrap"
        };

        await _applicationDbContext.AddUserAccountAsync(user, cancellationToken);
        await _applicationDbContext.AddRoleAsync(role, cancellationToken);
        await _applicationDbContext.AddUserRoleAsync(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            AssignedAtUtc = now,
            AssignedBy = "bootstrap"
        }, cancellationToken);

        foreach (var permissionCode in PermissionCodes.All)
        {
            var permission = new Permission
            {
                Code = permissionCode,
                Name = permissionCode,
                Description = "Seeded platform permission.",
                CreatedAtUtc = now,
                CreatedBy = "bootstrap"
            };

            await _applicationDbContext.AddPermissionAsync(permission, cancellationToken);
            await _applicationDbContext.AddRolePermissionAsync(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id,
                GrantedAtUtc = now,
                GrantedBy = "bootstrap"
            }, cancellationToken);
        }

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return new BootstrapAdminResponse
        {
            UserId = user.Id,
            UserName = user.UserName,
            RoleId = role.Id,
            RoleCode = role.Code,
            PermissionCodes = PermissionCodes.All
        };
    }

    private static void Validate(string userName, string email, string displayName, string password)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequiredAndLengthErrors(errors, "userName", userName, UserAccount.UserNameMaxLength);
        AddRequiredAndLengthErrors(errors, "email", email, UserAccount.EmailMaxLength);
        AddRequiredAndLengthErrors(errors, "displayName", displayName, UserAccount.DisplayNameMaxLength);

        if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
        {
            errors["email"] = ["Email format is invalid."];
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            errors["password"] = ["Password must be at least 8 characters."];
        }

        if (errors.Any())
        {
            throw new ValidationException("One or more validation errors occurred.", errors);
        }
    }

    private static void AddRequiredAndLengthErrors(
        IDictionary<string, string[]> errors,
        string key,
        string value,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors[key] = [$"{key} is required."];
            return;
        }

        if (value.Length > maxLength)
        {
            errors[key] = [$"{key} must be {maxLength} characters or fewer."];
        }
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var parsed = new MailAddress(email);
            return string.Equals(parsed.Address, email, StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
