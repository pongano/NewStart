using CoreProject.Backend.Application.AccessControl.RolePermissions.AssignPermissionToRole;
using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.RolePermissions;

public sealed class AssignPermissionToRoleCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldAssignPermissionToRole()
    {
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new Role { Id = roleId, Code = "admin", Name = "Administrator" });
        dbContext.Seed(new Permission { Id = permissionId, Code = "users.read", Name = "Read users" });

        var handler = new AssignPermissionToRoleCommandHandler(
            dbContext,
            new FakeDateTimeProvider(new DateTime(2026, 6, 10, 8, 30, 0, DateTimeKind.Utc)),
            new FakeCurrentUserService("tester", true));

        var response = await handler.HandleAsync(new AssignPermissionToRoleCommand
        {
            RoleId = roleId,
            PermissionId = permissionId
        });

        Assert.Equal(roleId, response.RoleId);
        Assert.Equal(permissionId, response.PermissionId);
        Assert.Equal("users.read", response.PermissionCode);
        Assert.Equal("tester", response.GrantedBy);
    }

    [Fact]
    public async Task HandleAsync_ShouldRejectDuplicateAssignment()
    {
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new Role { Id = roleId, Code = "admin", Name = "Administrator" });
        dbContext.Seed(new Permission { Id = permissionId, Code = "users.read", Name = "Read users" });
        dbContext.Seed(new RolePermission { RoleId = roleId, PermissionId = permissionId });

        var handler = new AssignPermissionToRoleCommandHandler(
            dbContext,
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => handler.HandleAsync(new AssignPermissionToRoleCommand
        {
            RoleId = roleId,
            PermissionId = permissionId
        }));

        Assert.NotNull(exception.Errors);
        Assert.True(exception.Errors.ContainsKey("rolePermission"));
    }
}
