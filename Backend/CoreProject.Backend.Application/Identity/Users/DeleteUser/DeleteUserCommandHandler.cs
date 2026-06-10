using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.Identity.Users.DeleteUser;

public sealed class DeleteUserCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public DeleteUserCommandHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<bool> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var userAccount = await _applicationDbContext.FindUserAccountByIdAsync(command.Id, cancellationToken);
        if (userAccount is null)
        {
            return false;
        }

        await _applicationDbContext.RemoveUserAccountAsync(userAccount, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
