using CoreProject.Backend.Application.AccessControl.Roles.DeleteRole;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Roles;

public sealed class DeleteRoleCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldDeleteExistingRole()
    {
        var role = new Role { Id = Guid.NewGuid(), Code = "ADMIN", Name = "Admin" };
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(role);

        var handler = new DeleteRoleCommandHandler(dbContext);

        var deleted = await handler.HandleAsync(new DeleteRoleCommand { Id = role.Id });

        Assert.True(deleted);
        Assert.Empty(await dbContext.ListRolesAsync());
    }
}
