using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.Identity.Auth.ChangePassword;

public sealed class ChangePasswordCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ChangePasswordCommandHandler(
        IApplicationDbContext applicationDbContext,
        ICurrentUserService currentUserService,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider)
    {
        _applicationDbContext = applicationDbContext;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            throw new ValidationException(
                "One or more validation errors occurred.",
                new Dictionary<string, string[]> { ["user"] = ["Authenticated user id is invalid."] });
        }

        var user = await _applicationDbContext.FindUserAccountByIdAsync(userId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return false;
        }

        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(command.NewPassword) || command.NewPassword.Length < 8)
        {
            errors["newPassword"] = ["Password must be at least 8 characters."];
        }

        if (string.IsNullOrWhiteSpace(command.CurrentPassword)
            || !_passwordHasher.VerifyPassword(user.PasswordHash, command.CurrentPassword))
        {
            errors["currentPassword"] = ["Current password is invalid."];
        }

        if (errors.Any())
        {
            throw new ValidationException("One or more validation errors occurred.", errors);
        }

        user.PasswordHash = _passwordHasher.HashPassword(command.NewPassword);
        user.LastModifiedAtUtc = _dateTimeProvider.UtcNow;
        user.LastModifiedBy = user.Id.ToString();

        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
