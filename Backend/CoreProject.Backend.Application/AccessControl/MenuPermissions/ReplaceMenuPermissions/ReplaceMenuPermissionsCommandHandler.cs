using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.AccessControl.MenuPermissions.ReplaceMenuPermissions;

public sealed class ReplaceMenuPermissionsCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public ReplaceMenuPermissionsCommandHandler(
        IApplicationDbContext applicationDbContext,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyCollection<MenuPermissionResponse>> HandleAsync(
        ReplaceMenuPermissionsCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var requestedPermissionIds = command.PermissionIds.Distinct().ToHashSet();
        var menu = await _applicationDbContext.FindMenuByIdAsync(command.MenuId, cancellationToken);
        var permissions = await _applicationDbContext.ListPermissionsAsync(cancellationToken);
        var permissionIds = permissions.Select(x => x.Id).ToHashSet();

        var errors = new Dictionary<string, string[]>();
        if (menu is null)
        {
            errors["menuId"] = ["Menu was not found."];
        }

        var missingPermissionIds = requestedPermissionIds.Where(x => !permissionIds.Contains(x)).ToList();
        if (missingPermissionIds.Any())
        {
            errors["permissionIds"] = ["One or more permissions were not found."];
        }

        if (errors.Any())
        {
            throw new ValidationException("One or more validation errors occurred.", errors);
        }

        var existingMenuPermissions = (await _applicationDbContext.ListMenuPermissionsAsync(cancellationToken))
            .Where(x => x.MenuId == command.MenuId)
            .ToList();

        foreach (var removed in existingMenuPermissions.Where(x => !requestedPermissionIds.Contains(x.PermissionId)))
        {
            await _applicationDbContext.RemoveMenuPermissionAsync(removed, cancellationToken);
        }

        var existingPermissionIds = existingMenuPermissions.Select(x => x.PermissionId).ToHashSet();
        foreach (var addedPermissionId in requestedPermissionIds.Where(x => !existingPermissionIds.Contains(x)))
        {
            await _applicationDbContext.AddMenuPermissionAsync(new MenuPermission
            {
                MenuId = command.MenuId,
                PermissionId = addedPermissionId,
                LinkedAtUtc = _dateTimeProvider.UtcNow,
                LinkedBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "system" : _currentUserService.UserId
            }, cancellationToken);
        }

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return (await _applicationDbContext.ListMenuPermissionsAsync(cancellationToken))
            .Where(x => x.MenuId == command.MenuId)
            .OrderBy(x => x.PermissionId)
            .Select(x => x.ToResponse())
            .ToList();
    }
}
