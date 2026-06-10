using CoreProject.Backend.Application.AccessControl.Roles.ListRoles;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Roles;

public sealed class ListRolesQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnRolesOrderedByCode()
    {
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(
            new Role { Code = "USER", Name = "User" },
            new Role { Code = "ADMIN", Name = "Administrator" });

        var handler = new ListRolesQueryHandler(dbContext);

        var response = await handler.HandleAsync(new ListRolesQuery());

        Assert.Collection(
            response,
            role => Assert.Equal("ADMIN", role.Code),
            role => Assert.Equal("USER", role.Code));
    }
}
