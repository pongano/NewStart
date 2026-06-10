using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Permissions;

public sealed class PermissionPersistenceContractTests
{
    [Fact]
    public async Task AddPermissionAsync_ShouldPersistPermissionAndFindById()
    {
        var dbContext = new FakeApplicationDbContext();
        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Code = "users.read",
            Name = "Read users",
            Description = "Allows viewing user records"
        };

        await dbContext.AddPermissionAsync(permission);

        var persistedPermission = await dbContext.FindPermissionByIdAsync(permission.Id);

        Assert.NotNull(persistedPermission);
        Assert.Equal("users.read", persistedPermission.Code);
        Assert.Equal("Read users", persistedPermission.Name);
        Assert.Equal("Allows viewing user records", persistedPermission.Description);
    }

    [Fact]
    public async Task PermissionCodeExistsAsync_ShouldDetectDuplicateCode()
    {
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new Permission
        {
            Code = "users.read",
            Name = "Read users"
        });

        var exists = await dbContext.PermissionCodeExistsAsync("users.read");
        var missing = await dbContext.PermissionCodeExistsAsync("users.write");

        Assert.True(exists);
        Assert.False(missing);
    }

    [Fact]
    public async Task ListPermissionsAsync_ShouldReturnPermissionsOrderedByCode()
    {
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(
            new Permission { Code = "users.write", Name = "Write users" },
            new Permission { Code = "menus.read", Name = "Read menus" },
            new Permission { Code = "users.read", Name = "Read users" });

        var permissions = await dbContext.ListPermissionsAsync();

        Assert.Collection(
            permissions,
            permission => Assert.Equal("menus.read", permission.Code),
            permission => Assert.Equal("users.read", permission.Code),
            permission => Assert.Equal("users.write", permission.Code));
    }
}
