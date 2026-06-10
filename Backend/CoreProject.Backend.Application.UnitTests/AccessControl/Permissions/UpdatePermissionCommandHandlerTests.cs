using CoreProject.Backend.Application.AccessControl.Permissions.UpdatePermission;
using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Permissions;

public sealed class UpdatePermissionCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldUpdatePermissionSuccessfully()
    {
        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Code = "users.read",
            Name = "Read users"
        };

        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(permission);

        var handler = new UpdatePermissionCommandHandler(
            dbContext,
            new FakeDateTimeProvider(new DateTime(2026, 6, 10, 8, 0, 0, DateTimeKind.Utc)),
            new FakeCurrentUserService("tester", true));

        var response = await handler.HandleAsync(new UpdatePermissionCommand
        {
            Id = permission.Id,
            Code = "users.manage",
            Name = "Manage users",
            Description = "Updated"
        });

        Assert.NotNull(response);
        Assert.Equal("users.manage", response.Code);
    }

    [Fact]
    public async Task HandleAsync_ShouldRejectDuplicateCode()
    {
        var existing = new Permission { Id = Guid.NewGuid(), Code = "users.read", Name = "Read" };
        var target = new Permission { Id = Guid.NewGuid(), Code = "users.write", Name = "Write" };
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(existing, target);

        var handler = new UpdatePermissionCommandHandler(
            dbContext,
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => handler.HandleAsync(new UpdatePermissionCommand
        {
            Id = target.Id,
            Code = "users.read",
            Name = "Updated"
        }));

        Assert.NotNull(exception.Errors);
        Assert.True(exception.Errors.ContainsKey("code"));
    }
}
