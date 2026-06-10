using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Roles;

public sealed class RolePersistenceContractTests
{
    [Fact]
    public async Task AddRoleAsync_ShouldPersistRoleAndFindById()
    {
        var dbContext = new FakeApplicationDbContext();
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Code = "ADMIN",
            Name = "Administrator",
            Description = "System administrator role",
            IsActive = true
        };

        await dbContext.AddRoleAsync(role);

        var persistedRole = await dbContext.FindRoleByIdAsync(role.Id);

        Assert.NotNull(persistedRole);
        Assert.Equal("ADMIN", persistedRole.Code);
        Assert.Equal("Administrator", persistedRole.Name);
        Assert.Equal("System administrator role", persistedRole.Description);
        Assert.True(persistedRole.IsActive);
    }

    [Fact]
    public async Task RoleCodeExistsAsync_ShouldDetectDuplicateCode()
    {
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new Role
        {
            Code = "ADMIN",
            Name = "Administrator"
        });

        var exists = await dbContext.RoleCodeExistsAsync("ADMIN");
        var missing = await dbContext.RoleCodeExistsAsync("AUDITOR");

        Assert.True(exists);
        Assert.False(missing);
    }

    [Fact]
    public async Task ListRolesAsync_ShouldReturnRolesOrderedByCode()
    {
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(
            new Role { Code = "USER", Name = "User" },
            new Role { Code = "ADMIN", Name = "Administrator" },
            new Role { Code = "AUDITOR", Name = "Auditor" });

        var roles = await dbContext.ListRolesAsync();

        Assert.Collection(
            roles,
            role => Assert.Equal("ADMIN", role.Code),
            role => Assert.Equal("AUDITOR", role.Code),
            role => Assert.Equal("USER", role.Code));
    }
}
