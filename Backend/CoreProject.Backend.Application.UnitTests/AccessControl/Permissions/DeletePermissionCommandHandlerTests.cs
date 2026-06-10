using CoreProject.Backend.Application.AccessControl.Permissions.DeletePermission;
using CoreProject.Backend.Application.UnitTests.Common.TestDoubles;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.UnitTests.AccessControl.Permissions;

public sealed class DeletePermissionCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldDeleteExistingPermission()
    {
        var permission = new Permission { Id = Guid.NewGuid(), Code = "users.read", Name = "Read" };
        var dbContext = new FakeApplicationDbContext();
        dbContext.Seed(permission);

        var handler = new DeletePermissionCommandHandler(dbContext);

        var deleted = await handler.HandleAsync(new DeletePermissionCommand { Id = permission.Id });

        Assert.True(deleted);
        Assert.Empty(await dbContext.ListPermissionsAsync());
    }
}
