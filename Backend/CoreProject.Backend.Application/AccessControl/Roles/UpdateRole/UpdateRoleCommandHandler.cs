using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;

namespace CoreProject.Backend.Application.AccessControl.Roles.UpdateRole;

public sealed class UpdateRoleCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public UpdateRoleCommandHandler(
        IApplicationDbContext applicationDbContext,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<RoleResponse?> HandleAsync(UpdateRoleCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var role = await _applicationDbContext.FindRoleByIdAsync(command.Id, cancellationToken);
        if (role is null)
        {
            return null;
        }

        var code = command.Code.Trim();
        var name = command.Name.Trim();
        var description = string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim();

        await ValidateAsync(command.Id, code, name, description, cancellationToken);

        role.Code = code;
        role.Name = name;
        role.Description = description;
        role.IsActive = command.IsActive;
        role.LastModifiedAtUtc = _dateTimeProvider.UtcNow;
        role.LastModifiedBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "system" : _currentUserService.UserId;

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return role.ToResponse();
    }

    private async Task ValidateAsync(Guid roleId, string code, string name, string? description, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        AddRequiredAndLengthErrors(errors, "code", code, Domain.AccessControl.Entities.Role.CodeMaxLength);
        AddRequiredAndLengthErrors(errors, "name", name, Domain.AccessControl.Entities.Role.NameMaxLength);

        if (!string.IsNullOrWhiteSpace(description) && description.Length > Domain.AccessControl.Entities.Role.DescriptionMaxLength)
        {
            errors["description"] = [$"description must be {Domain.AccessControl.Entities.Role.DescriptionMaxLength} characters or fewer."];
        }

        if (!errors.Any() && await _applicationDbContext.RoleCodeExistsAsync(code, roleId, cancellationToken))
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
