using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.AccessControl.Menus.CreateMenu;

public sealed class CreateMenuCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public CreateMenuCommandHandler(
        IApplicationDbContext applicationDbContext,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<MenuResponse> HandleAsync(
        CreateMenuCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var code = command.Code.Trim();
        var name = command.Name.Trim();
        var route = string.IsNullOrWhiteSpace(command.Route) ? null : command.Route.Trim();
        var icon = string.IsNullOrWhiteSpace(command.Icon) ? null : command.Icon.Trim();

        await ValidateAsync(code, name, route, icon, command.ParentId, cancellationToken);

        var menu = new Menu
        {
            Code = code,
            Name = name,
            Route = route,
            Icon = icon,
            SortOrder = command.SortOrder,
            IsVisible = command.IsVisible,
            ParentId = command.ParentId,
            CreatedAtUtc = _dateTimeProvider.UtcNow,
            CreatedBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "system" : _currentUserService.UserId
        };

        await _applicationDbContext.AddMenuAsync(menu, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return menu.ToResponse();
    }

    private async Task ValidateAsync(
        string code,
        string name,
        string? route,
        string? icon,
        Guid? parentId,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        AddRequiredAndLengthErrors(errors, "code", code, Menu.CodeMaxLength);
        AddRequiredAndLengthErrors(errors, "name", name, Menu.NameMaxLength);

        if (!string.IsNullOrWhiteSpace(route) && route.Length > Menu.RouteMaxLength)
        {
            errors["route"] = [$"route must be {Menu.RouteMaxLength} characters or fewer."];
        }

        if (!string.IsNullOrWhiteSpace(icon) && icon.Length > Menu.IconMaxLength)
        {
            errors["icon"] = [$"icon must be {Menu.IconMaxLength} characters or fewer."];
        }

        if (!errors.Any() && await _applicationDbContext.MenuCodeExistsAsync(code, cancellationToken))
        {
            errors["code"] = ["Code already exists."];
        }

        if (!errors.Any() && parentId.HasValue)
        {
            var parentMenu = await _applicationDbContext.FindMenuByIdAsync(parentId.Value, cancellationToken);
            if (parentMenu is null)
            {
                errors["parentId"] = ["Parent menu was not found."];
            }
        }

        if (errors.Any())
        {
            throw new ValidationException("One or more validation errors occurred.", errors);
        }
    }

    private static void AddRequiredAndLengthErrors(
        IDictionary<string, string[]> errors,
        string key,
        string value,
        int maxLength)
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
