using CoreProject.Backend.Application.AccessControl.MenuPermissions.AssignPermissionToMenu;
using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.MenuPermissions;

public sealed class AssignPermissionToMenuCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldAssignPermissionToMenu()
    {
        var menuId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new Menu { Id = menuId, Code = "dashboard", Name = "Dashboard" });
        dbContext.Seed(new Permission { Id = permissionId, Code = "dashboard.read", Name = "Read dashboard" });

        var handler = new AssignPermissionToMenuCommandHandler(
            dbContext,
            new FakeDateTimeProvider(new DateTime(2026, 6, 10, 6, 0, 0, DateTimeKind.Utc)),
            new FakeCurrentUserService("tester", true));

        var response = await handler.HandleAsync(new AssignPermissionToMenuCommand
        {
            MenuId = menuId,
            PermissionId = permissionId
        });

        Assert.Equal(menuId, response.MenuId);
        Assert.Equal(permissionId, response.PermissionId);
        Assert.Equal("tester", response.LinkedBy);
    }

    [Fact]
    public async Task HandleAsync_ShouldRejectDuplicateLink()
    {
        var menuId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new Menu { Id = menuId, Code = "dashboard", Name = "Dashboard" });
        dbContext.Seed(new Permission { Id = permissionId, Code = "dashboard.read", Name = "Read dashboard" });
        dbContext.Seed(new MenuPermission { MenuId = menuId, PermissionId = permissionId });

        var handler = new AssignPermissionToMenuCommandHandler(
            dbContext,
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => handler.HandleAsync(new AssignPermissionToMenuCommand
        {
            MenuId = menuId,
            PermissionId = permissionId
        }));

        Assert.NotNull(exception.Errors);
        Assert.True(exception.Errors.ContainsKey("menuPermission"));
    }
}
