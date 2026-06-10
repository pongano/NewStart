using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.Permissions.DeletePermission;

public sealed class DeletePermissionCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public DeletePermissionCommandHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<bool> HandleAsync(DeletePermissionCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var permission = await _applicationDbContext.FindPermissionByIdAsync(command.Id, cancellationToken);
        if (permission is null)
        {
            return false;
        }

        await _applicationDbContext.RemovePermissionAsync(permission, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
