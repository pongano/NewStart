using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.Roles.DeleteRole;

public sealed class DeleteRoleCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public DeleteRoleCommandHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<bool> HandleAsync(DeleteRoleCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var role = await _applicationDbContext.FindRoleByIdAsync(command.Id, cancellationToken);
        if (role is null)
        {
            return false;
        }

        await _applicationDbContext.RemoveRoleAsync(role, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
