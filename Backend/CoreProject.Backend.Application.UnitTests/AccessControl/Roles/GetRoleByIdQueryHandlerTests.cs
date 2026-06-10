using CoreProject.Backend.Application.AccessControl.Roles.GetRoleById;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Roles;

public sealed class GetRoleByIdQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnRoleWhenFound()
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Code = "ADMIN",
            Name = "Administrator"
        };

        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(role);

        var handler = new GetRoleByIdQueryHandler(dbContext);

        var response = await handler.HandleAsync(new GetRoleByIdQuery { Id = role.Id });

        Assert.NotNull(response);
        Assert.Equal(role.Id, response.Id);
        Assert.Equal("ADMIN", response.Code);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNullWhenMissing()
    {
        var handler = new GetRoleByIdQueryHandler(new FakeApplicationDbContext());

        var response = await handler.HandleAsync(new GetRoleByIdQuery { Id = Guid.NewGuid() });

        Assert.Null(response);
    }
}
