using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.AccessControl.Roles;

internal static class RoleMappings
{
    public static RoleResponse ToResponse(this Role role)
    {
        return new RoleResponse
        {
            Id = role.Id,
            Code = role.Code,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive,
            CreatedAtUtc = role.CreatedAtUtc
        };
    }
}
