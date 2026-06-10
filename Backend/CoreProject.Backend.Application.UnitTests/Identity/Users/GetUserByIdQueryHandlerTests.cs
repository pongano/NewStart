using CoreProject.Backend.Application.Identity.Users.GetUserById;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.Identity.Entities;

namespace CoreProject.Backend.Application.UnitTests.Identity.Users;

public sealed class GetUserByIdQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnUserWhenFound()
    {
        var userId = Guid.NewGuid();
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new UserAccount
        {
            Id = userId,
            UserName = "alice",
            Email = "alice@example.com",
            DisplayName = "Alice",
            CreatedAtUtc = new DateTime(2026, 6, 9, 0, 0, 0, DateTimeKind.Utc)
        });

        var handler = new GetUserByIdQueryHandler(dbContext);

        var response = await handler.HandleAsync(new GetUserByIdQuery { Id = userId });

        Assert.NotNull(response);
        Assert.Equal(userId, response.Id);
        Assert.Equal("alice", response.UserName);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNullWhenMissing()
    {
        var handler = new GetUserByIdQueryHandler(new FakeApplicationDbContext());

        var response = await handler.HandleAsync(new GetUserByIdQuery { Id = Guid.NewGuid() });

        Assert.Null(response);
    }
}
