using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Identity.Users.UpdateUser;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.Identity.Entities;

namespace CoreProject.Backend.Application.UnitTests.Identity.Users;

public sealed class UpdateUserCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldUpdateUserSuccessfully()
    {
        var userId = Guid.NewGuid();
        var utcNow = new DateTime(2026, 6, 10, 12, 0, 0, DateTimeKind.Utc);
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new UserAccount
        {
            Id = userId,
            UserName = "alice",
            Email = "alice@example.com",
            DisplayName = "Alice",
            IsActive = true
        });

        var handler = new UpdateUserCommandHandler(
            dbContext,
            new FakeDateTimeProvider(utcNow),
            new FakeCurrentUserService("tester", true));

        var response = await handler.HandleAsync(new UpdateUserCommand
        {
            Id = userId,
            UserName = "alice.updated",
            Email = "alice.updated@example.com",
            DisplayName = "Alice Updated",
            IsActive = false
        });

        Assert.NotNull(response);
        Assert.Equal("alice.updated", response.UserName);
        Assert.Equal("alice.updated@example.com", response.Email);
        Assert.False(response.IsActive);

        var persistedUser = await dbContext.FindUserAccountByIdAsync(userId);
        Assert.NotNull(persistedUser);
        Assert.Equal("tester", persistedUser.LastModifiedBy);
        Assert.Equal(utcNow, persistedUser.LastModifiedAtUtc);
    }

    [Fact]
    public async Task HandleAsync_ShouldRejectDuplicateUserName()
    {
        var targetUserId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(
            new UserAccount
            {
                Id = targetUserId,
                UserName = "alice",
                Email = "alice@example.com",
                DisplayName = "Alice"
            },
            new UserAccount
            {
                Id = Guid.NewGuid(),
                UserName = "bob",
                Email = "bob@example.com",
                DisplayName = "Bob"
            });

        var handler = new UpdateUserCommandHandler(
            dbContext,
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => handler.HandleAsync(new UpdateUserCommand
        {
            Id = targetUserId,
            UserName = "bob",
            Email = "alice.updated@example.com",
            DisplayName = "Alice Updated",
            IsActive = true
        }));

        Assert.NotNull(exception.Errors);
        Assert.True(exception.Errors.ContainsKey("userName"));
    }

    [Fact]
    public async Task HandleAsync_ShouldRejectDuplicateEmail()
    {
        var targetUserId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(
            new UserAccount
            {
                Id = targetUserId,
                UserName = "alice",
                Email = "alice@example.com",
                DisplayName = "Alice"
            },
            new UserAccount
            {
                Id = Guid.NewGuid(),
                UserName = "bob",
                Email = "bob@example.com",
                DisplayName = "Bob"
            });

        var handler = new UpdateUserCommandHandler(
            dbContext,
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => handler.HandleAsync(new UpdateUserCommand
        {
            Id = targetUserId,
            UserName = "alice.updated",
            Email = "bob@example.com",
            DisplayName = "Alice Updated",
            IsActive = true
        }));

        Assert.NotNull(exception.Errors);
        Assert.True(exception.Errors.ContainsKey("email"));
    }
}
