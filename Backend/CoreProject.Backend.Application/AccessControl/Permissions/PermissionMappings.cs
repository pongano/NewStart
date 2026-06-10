using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.AccessControl.Permissions;

internal static class PermissionMappings
{
    public static PermissionResponse ToResponse(this Permission permission)
    {
        return new PermissionResponse
        {
            Id = permission.Id,
            Code = permission.Code,
            Name = permission.Name,
            Description = permission.Description,
            CreatedAtUtc = permission.CreatedAtUtc
        };
    }
}
