using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.Permissions.UpdatePermission;

public sealed class UpdatePermissionCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public UpdatePermissionCommandHandler(
        IApplicationDbContext applicationDbContext,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<PermissionResponse?> HandleAsync(UpdatePermissionCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var permission = await _applicationDbContext.FindPermissionByIdAsync(command.Id, cancellationToken);
        if (permission is null)
        {
            return null;
        }

        var code = command.Code.Trim();
        var name = command.Name.Trim();
        var description = string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim();

        await ValidateAsync(command.Id, code, name, description, cancellationToken);

        permission.Code = code;
        permission.Name = name;
        permission.Description = description;
        permission.LastModifiedAtUtc = _dateTimeProvider.UtcNow;
        permission.LastModifiedBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "system" : _currentUserService.UserId;

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return permission.ToResponse();
    }

    private async Task ValidateAsync(Guid permissionId, string code, string name, string? description, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        AddRequiredAndLengthErrors(errors, "code", code, Domain.AccessControl.Entities.Permission.CodeMaxLength);
        AddRequiredAndLengthErrors(errors, "name", name, Domain.AccessControl.Entities.Permission.NameMaxLength);

        if (!string.IsNullOrWhiteSpace(description) && description.Length > Domain.AccessControl.Entities.Permission.DescriptionMaxLength)
        {
            errors["description"] = [$"description must be {Domain.AccessControl.Entities.Permission.DescriptionMaxLength} characters or fewer."];
        }

        if (!errors.Any() && await _applicationDbContext.PermissionCodeExistsAsync(code, permissionId, cancellationToken))
        {
            errors["code"] = ["Code already exists."];
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
