using CoreProject.Backend.Application.AccessControl.Permissions.CreatePermission;
using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Permissions;

public sealed class CreatePermissionCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreatePermissionSuccessfully()
    {
        var utcNow = new DateTime(2026, 6, 10, 3, 0, 0, DateTimeKind.Utc);
        var dbContext = new FakeApplicationDbContext();
        var handler = new CreatePermissionCommandHandler(
            dbContext,
            new FakeDateTimeProvider(utcNow),
            new FakeCurrentUserService("tester", true));

        var response = await handler.HandleAsync(new CreatePermissionCommand
        {
            Code = "users.read",
            Name = "Read users",
            Description = "Allows reading users"
        });

        Assert.Equal("users.read", response.Code);
        Assert.Equal(utcNow, response.CreatedAtUtc);

        var persisted = Assert.Single(await dbContext.ListPermissionsAsync());
        Assert.Equal("tester", persisted.CreatedBy);
    }

    [Fact]
    public async Task HandleAsync_ShouldRejectDuplicateCode()
    {
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new Permission { Code = "users.read", Name = "Existing" });

        var handler = new CreatePermissionCommandHandler(
            dbContext,
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => handler.HandleAsync(new CreatePermissionCommand
        {
            Code = "users.read",
            Name = "Duplicate"
        }));

        Assert.NotNull(exception.Errors);
        Assert.True(exception.Errors.ContainsKey("code"));
    }
}
