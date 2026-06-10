using CoreProject.Backend.Application.AccessControl.Permissions.GetPermissionById;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Permissions;

public sealed class GetPermissionByIdQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnPermissionWhenFound()
    {
        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Code = "users.read",
            Name = "Read users"
        };

        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(permission);

        var handler = new GetPermissionByIdQueryHandler(dbContext);

        var response = await handler.HandleAsync(new GetPermissionByIdQuery { Id = permission.Id });

        Assert.NotNull(response);
        Assert.Equal(permission.Id, response.Id);
        Assert.Equal(permission.Code, response.Code);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNullWhenMissing()
    {
        var handler = new GetPermissionByIdQueryHandler(new FakeApplicationDbContext());

        var response = await handler.HandleAsync(new GetPermissionByIdQuery { Id = Guid.NewGuid() });

        Assert.Null(response);
    }
}
