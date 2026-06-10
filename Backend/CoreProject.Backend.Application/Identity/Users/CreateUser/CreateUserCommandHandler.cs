using System.Net.Mail;
using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.Identity.Entities;

namespace CoreProject.Backend.Application.Identity.Users.CreateUser;

public sealed class CreateUserCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(
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

    public async Task<UserAccountResponse> HandleAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var userName = command.UserName.Trim();
        var email = command.Email.Trim();
        var displayName = command.DisplayName.Trim();

        await ValidateAsync(userName, email, displayName, command.Password, cancellationToken);

        var userAccount = new UserAccount
        {
            UserName = userName,
            Email = email,
            DisplayName = displayName,
            PasswordHash = _passwordHasher.HashPassword(command.Password),
            IsActive = command.IsActive,
            CreatedAtUtc = _dateTimeProvider.UtcNow,
            CreatedBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "system" : _currentUserService.UserId
        };

        await _applicationDbContext.AddUserAccountAsync(userAccount, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return userAccount.ToResponse();
    }

    private async Task ValidateAsync(
        string userName,
        string email,
        string displayName,
        string password,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        AddRequiredAndLengthErrors(errors, "userName", userName, UserAccount.UserNameMaxLength);
        AddRequiredAndLengthErrors(errors, "email", email, UserAccount.EmailMaxLength);
        AddRequiredAndLengthErrors(errors, "displayName", displayName, UserAccount.DisplayNameMaxLength);

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            errors["password"] = ["Password must be at least 8 characters."];
        }

        if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
        {
            errors["email"] = ["Email format is invalid."];
        }

        if (!errors.Any() && await _applicationDbContext.UserNameExistsAsync(userName, cancellationToken))
        {
            errors["userName"] = ["UserName already exists."];
        }

        if (!errors.Any() && await _applicationDbContext.EmailExistsAsync(email, cancellationToken))
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
