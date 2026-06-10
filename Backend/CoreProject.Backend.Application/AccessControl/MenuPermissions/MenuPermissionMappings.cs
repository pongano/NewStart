using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.AccessControl.MenuPermissions;

public static class MenuPermissionMappings
{
    public static MenuPermissionResponse ToResponse(this MenuPermission menuPermission)
    {
        return new MenuPermissionResponse
        {
            MenuId = menuPermission.MenuId,
            PermissionId = menuPermission.PermissionId,
            LinkedAtUtc = menuPermission.LinkedAtUtc,
            LinkedBy = menuPermission.LinkedBy
        };
    }
}
