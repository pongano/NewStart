using CoreProject.Backend.Application.AccessControl.RolePermissions.ListPermissionsByRole;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.RolePermissions;

public sealed class ListPermissionsByRoleQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnAssignedPermissionsOrderedByCode()
    {
        var roleId = Guid.NewGuid();
        var otherRoleId = Guid.NewGuid();
        var readPermissionId = Guid.NewGuid();
        var writePermissionId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(
            new Role { Id = roleId, Code = "admin", Name = "Administrator" },
            new Role { Id = otherRoleId, Code = "viewer", Name = "Viewer" });
        dbContext.Seed(
            new Permission { Id = writePermissionId, Code = "users.write", Name = "Write users" },
            new Permission { Id = readPermissionId, Code = "users.read", Name = "Read users" });
        dbContext.Seed(
            new RolePermission { RoleId = roleId, PermissionId = writePermissionId },
            new RolePermission { RoleId = roleId, PermissionId = readPermissionId },
            new RolePermission { RoleId = otherRoleId, PermissionId = readPermissionId });

        var handler = new ListPermissionsByRoleQueryHandler(dbContext);

        var response = await handler.HandleAsync(new ListPermissionsByRoleQuery { RoleId = roleId });

        Assert.Equal(2, response.Count);
        Assert.Equal(new[] { "users.read", "users.write" }, response.Select(x => x.PermissionCode).ToArray());
    }
}
