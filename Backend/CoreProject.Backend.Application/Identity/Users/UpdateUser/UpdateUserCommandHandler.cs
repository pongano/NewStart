using System.Net.Mail;
using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.Identity.Users.UpdateUser;

public sealed class UpdateUserCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateUserCommandHandler(
        IApplicationDbContext applicationDbContext,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService,
        IPasswordHasher passwordHasher)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserAccountResponse?> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var userAccount = await _applicationDbContext.FindUserAccountByIdAsync(command.Id, cancellationToken);
        if (userAccount is null)
        {
            return null;
        }

        var userName = command.UserName.Trim();
        var email = command.Email.Trim();
        var displayName = command.DisplayName.Trim();

        await ValidateAsync(command.Id, userName, email, displayName, command.Password, cancellationToken);

        userAccount.UserName = userName;
        userAccount.Email = email;
        userAccount.DisplayName = displayName;
        userAccount.IsActive = command.IsActive;
        if (!string.IsNullOrWhiteSpace(command.Password))
        {
            userAccount.PasswordHash = _passwordHasher.HashPassword(command.Password);
        }
        userAccount.LastModifiedAtUtc = _dateTimeProvider.UtcNow;
        userAccount.LastModifiedBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "system" : _currentUserService.UserId;

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return userAccount.ToResponse();
    }

    private async Task ValidateAsync(
        Guid userId,
        string userName,
        string email,
        string displayName,
        string? password,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        AddRequiredAndLengthErrors(errors, "userName", userName, Domain.Identity.Entities.UserAccount.UserNameMaxLength);
        AddRequiredAndLengthErrors(errors, "email", email, Domain.Identity.Entities.UserAccount.EmailMaxLength);
        AddRequiredAndLengthErrors(errors, "displayName", displayName, Domain.Identity.Entities.UserAccount.DisplayNameMaxLength);

        if (password is not null && (string.IsNullOrWhiteSpace(password) || password.Length < 8))
        {
            errors["password"] = ["Password must be at least 8 characters."];
        }

        if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
        {
            errors["email"] = ["Email format is invalid."];
        }

        if (!errors.Any() && await _applicationDbContext.UserNameExistsAsync(userName, userId, cancellationToken))
        {
            errors["userName"] = ["UserName already exists."];
        }

        if (!errors.Any() && await _applicationDbContext.EmailExistsAsync(email, userId, cancellationToken))
        {
            errors["email"] = ["Email already exists."];
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
