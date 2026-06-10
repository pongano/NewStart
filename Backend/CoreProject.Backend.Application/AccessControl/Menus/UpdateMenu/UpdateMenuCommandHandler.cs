using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.Menus.UpdateMenu;

public sealed class UpdateMenuCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public UpdateMenuCommandHandler(
        IApplicationDbContext applicationDbContext,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<MenuResponse?> HandleAsync(UpdateMenuCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var menu = await _applicationDbContext.FindMenuByIdAsync(command.Id, cancellationToken);
        if (menu is null)
        {
            return null;
        }

        var code = command.Code.Trim();
        var name = command.Name.Trim();
        var route = string.IsNullOrWhiteSpace(command.Route) ? null : command.Route.Trim();
        var icon = string.IsNullOrWhiteSpace(command.Icon) ? null : command.Icon.Trim();

        await ValidateAsync(command.Id, code, name, route, icon, command.ParentId, cancellationToken);

        menu.Code = code;
        menu.Name = name;
        menu.Route = route;
        menu.Icon = icon;
        menu.SortOrder = command.SortOrder;
        menu.IsVisible = command.IsVisible;
        menu.ParentId = command.ParentId;
        menu.LastModifiedAtUtc = _dateTimeProvider.UtcNow;
        menu.LastModifiedBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "system" : _currentUserService.UserId;

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return menu.ToResponse();
    }

    private async Task ValidateAsync(
        Guid menuId,
        string code,
        string name,
        string? route,
        string? icon,
        Guid? parentId,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        AddRequiredAndLengthErrors(errors, "code", code, Domain.AccessControl.Entities.Menu.CodeMaxLength);
        AddRequiredAndLengthErrors(errors, "name", name, Domain.AccessControl.Entities.Menu.NameMaxLength);

        if (!string.IsNullOrWhiteSpace(route) && route.Length > Domain.AccessControl.Entities.Menu.RouteMaxLength)
        {
            errors["route"] = [$"route must be {Domain.AccessControl.Entities.Menu.RouteMaxLength} characters or fewer."];
        }

        if (!string.IsNullOrWhiteSpace(icon) && icon.Length > Domain.AccessControl.Entities.Menu.IconMaxLength)
        {
            errors["icon"] = [$"icon must be {Domain.AccessControl.Entities.Menu.IconMaxLength} characters or fewer."];
        }

        if (!errors.Any() && await _applicationDbContext.MenuCodeExistsAsync(code, menuId, cancellationToken))
        {
            errors["code"] = ["Code already exists."];
        }

        if (!errors.Any() && parentId.HasValue)
        {
            if (parentId.Value == menuId)
            {
                errors["parentId"] = ["Menu cannot be its own parent."];
            }
            else
            {
                var parentMenu = await _applicationDbContext.FindMenuByIdAsync(parentId.Value, cancellationToken);
                if (parentMenu is null)
                {
                    errors["parentId"] = ["Parent menu was not found."];
                }
            }
        }

        if (errors.Any())
        {
            throw new ValidationException("One or more validation errors occurred.", errors);
        }
    }

    private static void AddRequiredAndLengthErrors(IDictionary<string, string[]> errors, string key, string value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors[key] = [$"{key} is required."];
            return;
        }

        if (value.Length > maxLength)
        {
            errors[key] = [$"{key} must be {maxLength} characters or fewer."];
        }
    }
}
