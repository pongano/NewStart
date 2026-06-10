using CoreProject.Backend.Application.AccessControl.Roles.CreateRole;
using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Roles;

public sealed class CreateRoleCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreateRoleSuccessfully()
    {
        var utcNow = new DateTime(2026, 6, 10, 2, 0, 0, DateTimeKind.Utc);
        var dbContext = new FakeApplicationDbContext();
        var handler = new CreateRoleCommandHandler(
            dbContext,
            new FakeDateTimeProvider(utcNow),
            new FakeCurrentUserService("tester", true));

        var response = await handler.HandleAsync(new CreateRoleCommand
        {
            Code = "ADMIN",
            Name = "Administrator",
            Description = "System administrator",
            IsActive = true
        });

        Assert.Equal("ADMIN", response.Code);
        Assert.Equal("Administrator", response.Name);
        Assert.True(response.IsActive);
        Assert.Equal(utcNow, response.CreatedAtUtc);

        var persistedRole = Assert.Single(await dbContext.ListRolesAsync());
        Assert.Equal("tester", persistedRole.CreatedBy);
        Assert.Equal(utcNow, persistedRole.CreatedAtUtc);
    }

    [Fact]
    public async Task HandleAsync_ShouldRejectDuplicateCode()
    {
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(new Role
        {
            Code = "ADMIN",
            Name = "Administrator"
        });

        var handler = new CreateRoleCommandHandler(
            dbContext,
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => handler.HandleAsync(new CreateRoleCommand
        {
            Code = "ADMIN",
            Name = "Another Admin"
        }));

        Assert.NotNull(exception.Errors);
        Assert.True(exception.Errors.ContainsKey("code"));
    }
}
