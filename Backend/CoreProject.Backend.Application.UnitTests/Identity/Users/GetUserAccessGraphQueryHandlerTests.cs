using CoreProject.Backend.Application.Identity.Users.GetUserAccessGraph;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;
using CoreProject.Backend.Domain.Identity.Entities;

namespace CoreProject.Backend.Application.UnitTests.Identity.Users;

public sealed class GetUserAccessGraphQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnRolesPermissionsAndMenusForUser()
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var menuId = Guid.NewGuid();
        var otherPermissionId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new UserAccount
        {
            Id = userId,
            UserName = "alice",
            Email = "alice@example.com",
            DisplayName = "Alice",
            IsActive = true
        });
        dbContext.Seed(new Role
        {
            Id = roleId,
            Code = "admin",
            Name = "Administrator",
            IsActive = true
        });
        dbContext.Seed(
            new Permission { Id = permissionId, Code = "users.read", Name = "Read users" },
            new Permission { Id = otherPermissionId, Code = "menus.read", Name = "Read menus" });
        dbContext.Seed(
            new Menu { Id = menuId, Code = "users", Name = "Users", SortOrder = 10 },
            new Menu { Id = Guid.NewGuid(), Code = "other", Name = "Other", SortOrder = 20 });
        dbContext.Seed(new UserRole { UserId = userId, RoleId = roleId });
        dbContext.Seed(new RolePermission { RoleId = roleId, PermissionId = permissionId });
        dbContext.Seed(new MenuPermission { MenuId = menuId, PermissionId = permissionId });

        var handler = new GetUserAccessGraphQueryHandler(dbContext);

        var response = await handler.HandleAsync(new GetUserAccessGraphQuery { UserId = userId });

        Assert.Equal(userId, response.UserId);
        Assert.Single(response.Roles);
        Assert.Equal("admin", response.Roles.Single().Code);
        Assert.Single(response.Permissions);
        Assert.Equal("users.read", response.Permissions.Single().Code);
        Assert.Single(response.Menus);
        Assert.Equal("users", response.Menus.Single().Code);
    }
}
