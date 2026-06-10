using CoreProject.Backend.Application.AccessControl.UserRoles.RemoveRoleFromUser;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.UserRoles;

public sealed class RemoveRoleFromUserCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldRemoveExistingAssignment()
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new UserRole { UserId = userId, RoleId = roleId });

        var handler = new RemoveRoleFromUserCommandHandler(dbContext);

        var removed = await handler.HandleAsync(new RemoveRoleFromUserCommand
        {
            UserId = userId,
            RoleId = roleId
        });

        Assert.True(removed);
        Assert.Empty(await dbContext.ListUserRolesAsync());
    }
}
