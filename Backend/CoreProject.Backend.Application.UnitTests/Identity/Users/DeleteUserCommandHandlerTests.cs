using CoreProject.Backend.Application.Identity.Users.DeleteUser;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.Identity.Entities;

namespace CoreProject.Backend.Application.UnitTests.Identity.Users;

public sealed class DeleteUserCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldDeleteExistingUser()
    {
        var userId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new UserAccount
        {
            Id = userId,
            UserName = "alice",
            Email = "alice@example.com",
            DisplayName = "Alice"
        });

        var handler = new DeleteUserCommandHandler(dbContext);

        var deleted = await handler.HandleAsync(new DeleteUserCommand { Id = userId });

        Assert.True(deleted);
        Assert.Empty(await dbContext.ListUserAccountsAsync());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFalseWhenMissing()
    {
        var handler = new DeleteUserCommandHandler(new FakeApplicationDbContext());

        var deleted = await handler.HandleAsync(new DeleteUserCommand { Id = Guid.NewGuid() });

        Assert.False(deleted);
    }
}
