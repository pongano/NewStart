using CoreProject.Backend.Application.AccessControl.MenuPermissions.ListPermissionsByMenu;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.MenuPermissions;

public sealed class ListPermissionsByMenuQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPermissionsForRequestedMenuOnly()
    {
        var menuId = Guid.NewGuid();
        var otherMenuId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new Menu { Id = menuId, Code = "dashboard", Name = "Dashboard" });
        dbContext.Seed(new Menu { Id = otherMenuId, Code = "users", Name = "Users" });
        dbContext.Seed(
            new MenuPermission { MenuId = menuId, PermissionId = Guid.Parse("11111111-1111-1111-1111-111111111111") },
            new MenuPermission { MenuId = otherMenuId, PermissionId = Guid.Parse("22222222-2222-2222-2222-222222222222") });

        var handler = new ListPermissionsByMenuQueryHandler(dbContext);

        var response = await handler.HandleAsync(new ListPermissionsByMenuQuery { MenuId = menuId });

        var link = Assert.Single(response);
        Assert.Equal(menuId, link.MenuId);
    }
}
