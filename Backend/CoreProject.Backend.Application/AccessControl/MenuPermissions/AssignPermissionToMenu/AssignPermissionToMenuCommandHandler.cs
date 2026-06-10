using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.AccessControl.MenuPermissions.AssignPermissionToMenu;

public sealed class AssignPermissionToMenuCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public AssignPermissionToMenuCommandHandler(
        IApplicationDbContext applicationDbContext,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<MenuPermissionResponse> HandleAsync(
        AssignPermissionToMenuCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        await ValidateAsync(command.MenuId, command.PermissionId, cancellationToken);

        var menuPermission = new MenuPermission
        {
            MenuId = command.MenuId,
            PermissionId = command.PermissionId,
            LinkedAtUtc = _dateTimeProvider.UtcNow,
            LinkedBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "system" : _currentUserService.UserId
        };

        await _applicationDbContext.AddMenuPermissionAsync(menuPermission, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return menuPermission.ToResponse();
    }

    private async Task ValidateAsync(Guid menuId, Guid permissionId, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        var menu = await _applicationDbContext.FindMenuByIdAsync(menuId, cancellationToken);
        if (menu is null)
        {
            errors["menuId"] = ["Menu was not found."];
        }

        var permission = await _applicationDbContext.FindPermissionByIdAsync(permissionId, cancellationToken);
        if (permission is null)
        {
            errors["permissionId"] = ["Permission was not found."];
        }

        if (!errors.Any() && await _applicationDbContext.MenuPermissionExistsAsync(menuId, permissionId, cancellationToken))
        {
            errors["menuPermission"] = ["Permission is already linked to the menu."];
        }

        if (errors.Any())
        {
            throw new ValidationException("One or more validation errors occurred.", errors);
        }
    }
}
