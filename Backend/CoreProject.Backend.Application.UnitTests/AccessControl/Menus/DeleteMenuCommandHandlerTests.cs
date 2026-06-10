using CoreProject.Backend.Application.AccessControl.Menus.DeleteMenu;
using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Menus;

public sealed class DeleteMenuCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldDeleteLeafMenu()
    {
        var menu = new Menu { Id = Guid.NewGuid(), Code = "dashboard", Name = "Dashboard" };
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(menu);

        var handler = new DeleteMenuCommandHandler(dbContext);

        var deleted = await handler.HandleAsync(new DeleteMenuCommand { Id = menu.Id });

        Assert.True(deleted);
        Assert.Empty(await dbContext.ListMenusAsync());
    }

    [Fact]
    public async Task HandleAsync_ShouldRejectMenuWithChildren()
    {
        var parentId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(
            new Menu { Id = parentId, Code = "parent", Name = "Parent" },
            new Menu { Id = Guid.NewGuid(), Code = "child", Name = "Child", ParentId = parentId });

        var handler = new DeleteMenuCommandHandler(dbContext);

        await Assert.ThrowsAsync<ValidationException>(() => handler.HandleAsync(new DeleteMenuCommand { Id = parentId }));
    }
}
