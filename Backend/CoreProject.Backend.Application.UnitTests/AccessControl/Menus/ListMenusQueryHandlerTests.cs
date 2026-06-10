using CoreProject.Backend.Application.AccessControl.Menus.ListMenus;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Menus;

public sealed class ListMenusQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnMenusOrderedBySortOrderThenCode()
    {
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(
            new Menu { Code = "users", Name = "Users", SortOrder = 20 },
            new Menu { Code = "dashboard", Name = "Dashboard", SortOrder = 10 });

        var handler = new ListMenusQueryHandler(dbContext);

        var response = await handler.HandleAsync(new ListMenusQuery());

        Assert.Collection(
            response,
            menu => Assert.Equal("dashboard", menu.Code),
            menu => Assert.Equal("users", menu.Code));
    }
}
