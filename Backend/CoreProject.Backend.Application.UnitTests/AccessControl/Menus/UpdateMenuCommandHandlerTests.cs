using CoreProject.Backend.Application.AccessControl.Menus.UpdateMenu;
using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Menus;

public sealed class UpdateMenuCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldUpdateMenuSuccessfully()
    {
        var menu = new Menu
        {
            Id = Guid.NewGuid(),
            Code = "dashboard",
            Name = "Dashboard"
        };

        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(menu);

        var handler = new UpdateMenuCommandHandler(
            dbContext,
            new FakeDateTimeProvider(new DateTime(2026, 6, 10, 9, 0, 0, DateTimeKind.Utc)),
            new FakeCurrentUserService("tester", true));

        var response = await handler.HandleAsync(new UpdateMenuCommand
        {
            Id = menu.Id,
            Code = "home",
            Name = "Home",
            Route = "/home",
            SortOrder = 5,
            IsVisible = false
        });

        Assert.NotNull(response);
        Assert.Equal("home", response.Code);
        Assert.False(response.IsVisible);
    }

    [Fact]
    public async Task HandleAsync_ShouldRejectSelfParent()
    {
        var menu = new Menu
        {
            Id = Guid.NewGuid(),
            Code = "dashboard",
            Name = "Dashboard"
        };

        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(menu);

        var handler = new UpdateMenuCommandHandler(
            dbContext,
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => handler.HandleAsync(new UpdateMenuCommand
        {
            Id = menu.Id,
            Code = "dashboard",
            Name = "Dashboard",
            ParentId = menu.Id
        }));

        Assert.NotNull(exception.Errors);
        Assert.True(exception.Errors.ContainsKey("parentId"));
    }
}
