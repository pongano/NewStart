using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.Menus.DeleteMenu;

public sealed class DeleteMenuCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;

    public DeleteMenuCommandHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<bool> HandleAsync(DeleteMenuCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var menu = await _applicationDbContext.FindMenuByIdAsync(command.Id, cancellationToken);
        if (menu is null)
        {
            return false;
        }

        if (await _applicationDbContext.MenuHasChildrenAsync(command.Id, cancellationToken))
        {
            throw new ValidationException(
                "One or more validation errors occurred.",
                new Dictionary<string, string[]> { ["menu"] = ["Menu cannot be deleted while it still has child menus."] });
        }

        await _applicationDbContext.RemoveMenuAsync(menu, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
