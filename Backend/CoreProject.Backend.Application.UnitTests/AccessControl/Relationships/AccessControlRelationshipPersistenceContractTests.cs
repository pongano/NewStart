using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Relationships;

public sealed class AccessControlRelationshipPersistenceContractTests
{
    [Fact]
    public async Task AddUserRoleAsync_ShouldPersistAndDetectAssignment()
    {
        var dbContext = new FakeApplicationDbContext();
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        await dbContext.AddUserRoleAsync(new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAtUtc = new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc),
            AssignedBy = "system"
        });

        var exists = await dbContext.UserRoleExistsAsync(userId, roleId);
        var userRoles = await dbContext.ListUserRolesAsync();

        Assert.True(exists);
        var persisted = Assert.Single(userRoles);
        Assert.Equal(userId, persisted.UserId);
        Assert.Equal(roleId, persisted.RoleId);
        Assert.Equal("system", persisted.AssignedBy);
    }

    [Fact]
    public async Task AddRolePermissionAsync_ShouldPersistAndDetectGrant()
    {
        var dbContext = new FakeApplicationDbContext();
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        await dbContext.AddRolePermissionAsync(new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            GrantedAtUtc = new DateTime(2026, 6, 10, 1, 0, 0, DateTimeKind.Utc),
            GrantedBy = "system"
        });

        var exists = await dbContext.RolePermissionExistsAsync(roleId, permissionId);
        var rolePermissions = await dbContext.ListRolePermissionsAsync();

        Assert.True(exists);
        var persisted = Assert.Single(rolePermissions);
        Assert.Equal(roleId, persisted.RoleId);
        Assert.Equal(permissionId, persisted.PermissionId);
        Assert.Equal("system", persisted.GrantedBy);
    }
}
