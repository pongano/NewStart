using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.AccessControl.Permissions.CreatePermission;

public sealed class CreatePermissionCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public CreatePermissionCommandHandler(
        IApplicationDbContext applicationDbContext,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<PermissionResponse> HandleAsync(
        CreatePermissionCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var code = command.Code.Trim();
        var name = command.Name.Trim();
        var description = string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim();

        await ValidateAsync(code, name, description, cancellationToken);

        var permission = new Permission
        {
            Code = code,
            Name = name,
            Description = description,
            CreatedAtUtc = _dateTimeProvider.UtcNow,
            CreatedBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "system" : _currentUserService.UserId
        };

        await _applicationDbContext.AddPermissionAsync(permission, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return permission.ToResponse();
    }

    private async Task ValidateAsync(
        string code,
        string name,
        string? description,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        AddRequiredAndLengthErrors(errors, "code", code, Permission.CodeMaxLength);
        AddRequiredAndLengthErrors(errors, "name", name, Permission.NameMaxLength);

        if (!string.IsNullOrWhiteSpace(description) && description.Length > Permission.DescriptionMaxLength)
        {
            errors["description"] = [$"description must be {Permission.DescriptionMaxLength} characters or fewer."];
        }

        if (!errors.Any() && await _applicationDbContext.PermissionCodeExistsAsync(code, cancellationToken))
        {
            errors["code"] = ["Code already exists."];
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
