using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.Identity.Users.ResetUserPassword;

public sealed class ResetUserPasswordCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public ResetUserPasswordCommandHandler(
        IApplicationDbContext applicationDbContext,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _applicationDbContext = applicationDbContext;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<bool> HandleAsync(ResetUserPasswordCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (string.IsNullOrWhiteSpace(command.NewPassword) || command.NewPassword.Length < 8)
        {
            throw new ValidationException(
                "One or more validation errors occurred.",
                new Dictionary<string, string[]> { ["newPassword"] = ["Password must be at least 8 characters."] });
        }

        var user = await _applicationDbContext.FindUserAccountByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return false;
        }

        user.PasswordHash = _passwordHasher.HashPassword(command.NewPassword);
        user.LastModifiedAtUtc = _dateTimeProvider.UtcNow;
        user.LastModifiedBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "system" : _currentUserService.UserId;

        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
