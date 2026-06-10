using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.MenuPermissions;

public sealed class MenuPermissionPersistenceContractTests
{
    [Fact]
    public async Task AddMenuPermissionAsync_ShouldPersistAndDetectLink()
    {
        var dbContext = new FakeApplicationDbContext();
        var menuId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        await dbContext.AddMenuPermissionAsync(new MenuPermission
        {
            MenuId = menuId,
            PermissionId = permissionId,
            LinkedAtUtc = new DateTime(2026, 6, 10, 5, 0, 0, DateTimeKind.Utc),
            LinkedBy = "system"
        });

        var exists = await dbContext.MenuPermissionExistsAsync(menuId, permissionId);
        var links = await dbContext.ListMenuPermissionsAsync();

        Assert.True(exists);
        var persisted = Assert.Single(links);
        Assert.Equal(menuId, persisted.MenuId);
        Assert.Equal(permissionId, persisted.PermissionId);
    }
}
