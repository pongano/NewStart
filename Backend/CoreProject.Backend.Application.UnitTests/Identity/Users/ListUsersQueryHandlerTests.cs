using CoreProject.Backend.Application.Identity.Users.ListUsers;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.Identity.Entities;

namespace CoreProject.Backend.Application.UnitTests.Identity.Users;

public sealed class ListUsersQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnUsersOrderedByUserName()
    {
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(
            new UserAccount
            {
                UserName = "charlie",
                Email = "charlie@example.com",
                DisplayName = "Charlie"
            },
            new UserAccount
            {
                UserName = "alice",
                Email = "alice@example.com",
                DisplayName = "Alice"
            });

        var handler = new ListUsersQueryHandler(dbContext);

        var response = await handler.HandleAsync(new ListUsersQuery());

        Assert.Collection(
            response,
            first => Assert.Equal("alice", first.UserName),
            second => Assert.Equal("charlie", second.UserName));
    }
}
