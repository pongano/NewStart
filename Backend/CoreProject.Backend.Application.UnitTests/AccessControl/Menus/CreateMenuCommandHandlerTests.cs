using CoreProject.Backend.Application.AccessControl.Menus.CreateMenu;
using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Menus;

public sealed class CreateMenuCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreateMenuSuccessfully()
    {
        var utcNow = new DateTime(2026, 6, 10, 4, 0, 0, DateTimeKind.Utc);
        var dbContext = new FakeApplicationDbContext();
        var handler = new CreateMenuCommandHandler(
            dbContext,
            new FakeDateTimeProvider(utcNow),
            new FakeCurrentUserService("tester", true));

        var response = await handler.HandleAsync(new CreateMenuCommand
        {
            Code = "dashboard",
            Name = "Dashboard",
            Route = "/dashboard",
            SortOrder = 10
        });

        Assert.Equal("dashboard", response.Code);
        Assert.Equal("/dashboard", response.Route);
        Assert.Equal(utcNow, response.CreatedAtUtc);

        var persisted = Assert.Single(await dbContext.ListMenusAsync());
        Assert.Equal("tester", persisted.CreatedBy);
    }

    [Fact]
    public async Task HandleAsync_ShouldRejectUnknownParentMenu()
    {
        var handler = new CreateMenuCommandHandler(
            new FakeApplicationDbContext(),
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => handler.HandleAsync(new CreateMenuCommand
        {
            Code = "users",
            Name = "Users",
            ParentId = Guid.NewGuid()
        }));

        Assert.NotNull(exception.Errors);
        Assert.True(exception.Errors.ContainsKey("parentId"));
    }
}
