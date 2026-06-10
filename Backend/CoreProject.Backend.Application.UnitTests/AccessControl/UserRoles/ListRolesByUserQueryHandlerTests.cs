using CoreProject.Backend.Application.AccessControl.UserRoles.ListRolesByUser;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;
using CoreProject.Backend.Domain.Identity.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.UserRoles;

public sealed class ListRolesByUserQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnAssignedRolesOrderedByCode()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();
        var viewerRoleId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(
            new UserAccount { Id = userId, UserName = "alice", Email = "alice@example.com", DisplayName = "Alice" },
            new UserAccount { Id = otherUserId, UserName = "bob", Email = "bob@example.com", DisplayName = "Bob" });
        dbContext.Seed(
            new Role { Id = viewerRoleId, Code = "viewer", Name = "Viewer" },
            new Role { Id = adminRoleId, Code = "admin", Name = "Administrator" });
        dbContext.Seed(
            new UserRole { UserId = userId, RoleId = viewerRoleId },
            new UserRole { UserId = userId, RoleId = adminRoleId },
            new UserRole { UserId = otherUserId, RoleId = viewerRoleId });

        var handler = new ListRolesByUserQueryHandler(dbContext);

        var response = await handler.HandleAsync(new ListRolesByUserQuery { UserId = userId });

        Assert.Equal(2, response.Count);
        Assert.Equal(new[] { "admin", "viewer" }, response.Select(x => x.RoleCode).ToArray());
    }
}
