using CoreProject.Backend.Application.AccessControl.UserRoles.AssignRoleToUser;
using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;
using CoreProject.Backend.Domain.Identity.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.UserRoles;

public sealed class AssignRoleToUserCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldAssignRoleToUser()
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new UserAccount
        {
            Id = userId,
            UserName = "alice",
            Email = "alice@example.com",
            DisplayName = "Alice"
        });
        dbContext.Seed(new Role
        {
            Id = roleId,
            Code = "admin",
            Name = "Administrator",
            IsActive = true
        });

        var handler = new AssignRoleToUserCommandHandler(
            dbContext,
            new FakeDateTimeProvider(new DateTime(2026, 6, 10, 8, 0, 0, DateTimeKind.Utc)),
            new FakeCurrentUserService("tester", true));

        var response = await handler.HandleAsync(new AssignRoleToUserCommand
        {
            UserId = userId,
            RoleId = roleId
        });

        Assert.Equal(userId, response.UserId);
        Assert.Equal(roleId, response.RoleId);
        Assert.Equal("admin", response.RoleCode);
        Assert.Equal("tester", response.AssignedBy);
    }

    [Fact]
    public async Task HandleAsync_ShouldRejectDuplicateAssignment()
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new UserAccount
        {
            Id = userId,
            UserName = "alice",
            Email = "alice@example.com",
            DisplayName = "Alice"
        });
        dbContext.Seed(new Role
        {
            Id = roleId,
            Code = "admin",
            Name = "Administrator"
        });
        dbContext.Seed(new UserRole
        {
            UserId = userId,
            RoleId = roleId
        });

        var handler = new AssignRoleToUserCommandHandler(
            dbContext,
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => handler.HandleAsync(new AssignRoleToUserCommand
        {
            UserId = userId,
            RoleId = roleId
        }));

        Assert.NotNull(exception.Errors);
        Assert.True(exception.Errors.ContainsKey("userRole"));
    }
}
