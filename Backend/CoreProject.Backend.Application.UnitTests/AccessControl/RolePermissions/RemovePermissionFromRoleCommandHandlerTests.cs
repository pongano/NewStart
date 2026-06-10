using CoreProject.Backend.Application.AccessControl.RolePermissions.RemovePermissionFromRole;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.RolePermissions;

public sealed class RemovePermissionFromRoleCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldRemoveExistingAssignment()
    {
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new RolePermission { RoleId = roleId, PermissionId = permissionId });

        var handler = new RemovePermissionFromRoleCommandHandler(dbContext);

        var removed = await handler.HandleAsync(new RemovePermissionFromRoleCommand
        {
            RoleId = roleId,
            PermissionId = permissionId
        });

        Assert.True(removed);
        Assert.Empty(await dbContext.ListRolePermissionsAsync());
    }
}
