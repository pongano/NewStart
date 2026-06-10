using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Menus;

public sealed class MenuPersistenceContractTests
{
    [Fact]
    public async Task AddMenuAsync_ShouldPersistMenuAndFindById()
    {
        var dbContext = new FakeApplicationDbContext();
        var parentId = Guid.NewGuid();
        var menu = new Menu
        {
            Id = Guid.NewGuid(),
            Code = "users",
            Name = "Users",
            Route = "/users",
            Icon = "users",
            SortOrder = 20,
            IsVisible = true,
            ParentId = parentId
        };

        await dbContext.AddMenuAsync(menu);

        var persistedMenu = await dbContext.FindMenuByIdAsync(menu.Id);

        Assert.NotNull(persistedMenu);
        Assert.Equal("users", persistedMenu.Code);
        Assert.Equal("/users", persistedMenu.Route);
        Assert.Equal("users", persistedMenu.Icon);
        Assert.Equal(20, persistedMenu.SortOrder);
        Assert.Equal(parentId, persistedMenu.ParentId);
    }

    [Fact]
    public async Task MenuCodeExistsAsync_ShouldDetectDuplicateCode()
    {
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new Menu
        {
            Code = "dashboard",
            Name = "Dashboard"
        });

        var exists = await dbContext.MenuCodeExistsAsync("dashboard");
        var missing = await dbContext.MenuCodeExistsAsync("settings");

        Assert.True(exists);
        Assert.False(missing);
    }

    [Fact]
    public async Task ListMenusAsync_ShouldReturnMenusOrderedBySortOrderThenCode()
    {
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(
            new Menu { Code = "users", Name = "Users", SortOrder = 20 },
            new Menu { Code = "dashboard", Name = "Dashboard", SortOrder = 10 },
            new Menu { Code = "audit", Name = "Audit", SortOrder = 20 });

        var menus = await dbContext.ListMenusAsync();

        Assert.Collection(
            menus,
            menu => Assert.Equal("dashboard", menu.Code),
            menu => Assert.Equal("audit", menu.Code),
            menu => Assert.Equal("users", menu.Code));
    }
}
