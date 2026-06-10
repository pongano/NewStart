using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Identity.Users.CreateUser;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.Identity.Entities;

namespace CoreProject.Backend.Application.UnitTests.Identity.Users;

public sealed class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreateUserSuccessfully()
    {
        var utcNow = new DateTime(2026, 6, 9, 0, 0, 0, DateTimeKind.Utc);
        var dbContext = new FakeApplicationDbContext();
        var handler = new CreateUserCommandHandler(
            dbContext,
            new FakeDateTimeProvider(utcNow),
            new FakeCurrentUserService("tester", true),
            new FakePasswordHasher());

        var response = await handler.HandleAsync(new CreateUserCommand
        {
            UserName = "alice",
            Email = "alice@example.com",
            DisplayName = "Alice",
            Password = "Password123!",
            IsActive = true
        });

        Assert.Equal("alice", response.UserName);
        Assert.Equal("alice@example.com", response.Email);
        Assert.Equal("Alice", response.DisplayName);
        Assert.True(response.IsActive);
        Assert.Equal(utcNow, response.CreatedAtUtc);

        var persistedUser = Assert.Single(await dbContext.ListUserAccountsAsync());
        Assert.Equal("tester", persistedUser.CreatedBy);
        Assert.Equal(utcNow, persistedUser.CreatedAtUtc);
        Assert.Equal("hashed:Password123!", persistedUser.PasswordHash);
    }

    [Fact]
    public async Task HandleAsync_ShouldRejectDuplicateUserName()
    {
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new UserAccount
        {
            UserName = "alice",
            Email = "existing@example.com",
            DisplayName = "Existing User"
        });

        var handler = new CreateUserCommandHandler(
            dbContext,
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService(),
            new FakePasswordHasher());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => handler.HandleAsync(new CreateUserCommand
        {
            UserName = "alice",
            Email = "alice@example.com",
            DisplayName = "Alice",
            Password = "Password123!"
        }));

        Assert.NotNull(exception.Errors);
        Assert.True(exception.Errors.ContainsKey("userName"));
    }

    [Fact]
    public async Task HandleAsync_ShouldRejectDuplicateEmail()
    {
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new UserAccount
        {
            UserName = "alice",
            Email = "alice@example.com",
            DisplayName = "Existing User"
        });

        var handler = new CreateUserCommandHandler(
            dbContext,
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService(),
            new FakePasswordHasher());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => handler.HandleAsync(new CreateUserCommand
        {
            UserName = "new-alice",
            Email = "alice@example.com",
            DisplayName = "Alice",
            Password = "Password123!"
        }));

        Assert.NotNull(exception.Errors);
        Assert.True(exception.Errors.ContainsKey("email"));
    }
}
