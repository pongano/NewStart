using CoreProject.Backend.Application.AccessControl.Permissions.ListPermissions;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Permissions;

public sealed class ListPermissionsQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPermissionsOrderedByCode()
    {
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(
            new Permission { Code = "users.write", Name = "Write users" },
            new Permission { Code = "users.read", Name = "Read users" });

        var handler = new ListPermissionsQueryHandler(dbContext);

        var response = await handler.HandleAsync(new ListPermissionsQuery());

        Assert.Collection(
            response,
            permission => Assert.Equal("users.read", permission.Code),
            permission => Assert.Equal("users.write", permission.Code));
    }
}
