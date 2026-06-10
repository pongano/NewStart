using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.UserRoles.RemoveRoleFromUser;

public sealed class RemoveRoleFromUserCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public RemoveRoleFromUserCommandHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<bool> HandleAsync(RemoveRoleFromUserCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var userRole = await _applicationDbContext.FindUserRoleAsync(command.UserId, command.RoleId, cancellationToken);
        if (userRole is null)
        {
            return false;
        }

        await _applicationDbContext.RemoveUserRoleAsync(userRole, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
