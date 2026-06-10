using CoreProject.Backend.Application.AccessControl.Menus.GetMenuById;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Menus;

public sealed class GetMenuByIdQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnMenuWhenFound()
    {
        var menu = new Menu
        {
            Id = Guid.NewGuid(),
            Code = "dashboard",
            Name = "Dashboard"
        };

        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(menu);

        var handler = new GetMenuByIdQueryHandler(dbContext);

        var response = await handler.HandleAsync(new GetMenuByIdQuery { Id = menu.Id });

        Assert.NotNull(response);
        Assert.Equal(menu.Id, response.Id);
        Assert.Equal(menu.Code, response.Code);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNullWhenMissing()
    {
        var handler = new GetMenuByIdQueryHandler(new FakeApplicationDbContext());

        var response = await handler.HandleAsync(new GetMenuByIdQuery { Id = Guid.NewGuid() });

        Assert.Null(response);
    }
}
