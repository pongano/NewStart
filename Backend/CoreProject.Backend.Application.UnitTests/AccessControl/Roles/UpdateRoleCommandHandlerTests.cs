using CoreProject.Backend.Application.AccessControl.Roles.UpdateRole;
using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Roles;

public sealed class UpdateRoleCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldUpdateRoleSuccessfully()
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Code = "ADMIN",
            Name = "Administrator"
        };

        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(role);

        var handler = new UpdateRoleCommandHandler(
            dbContext,
            new FakeDateTimeProvider(new DateTime(2026, 6, 10, 7, 0, 0, DateTimeKind.Utc)),
            new FakeCurrentUserService("tester", true));

        var response = await handler.HandleAsync(new UpdateRoleCommand
        {
            Id = role.Id,
            Code = "SUPERADMIN",
            Name = "Super Administrator",
            Description = "Updated",
            IsActive = false
        });

        Assert.NotNull(response);
        Assert.Equal("SUPERADMIN", response.Code);
        Assert.False(response.IsActive);
    }

    [Fact]
    public async Task HandleAsync_ShouldRejectDuplicateCode()
    {
        var existing = new Role { Id = Guid.NewGuid(), Code = "ADMIN", Name = "Admin" };
        var target = new Role { Id = Guid.NewGuid(), Code = "USER", Name = "User" };
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(existing, target);

        var handler = new UpdateRoleCommandHandler(
            dbContext,
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => handler.HandleAsync(new UpdateRoleCommand
        {
            Id = target.Id,
            Code = "ADMIN",
            Name = "Updated"
        }));

        Assert.NotNull(exception.Errors);
        Assert.True(exception.Errors.ContainsKey("code"));
    }
}
