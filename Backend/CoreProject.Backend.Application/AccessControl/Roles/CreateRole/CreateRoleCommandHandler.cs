using CoreProject.Backend.Application.Common.Exceptions;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.AccessControl.Entities;

namespace CoreProject.Backend.Application.AccessControl.Roles.CreateRole;

public sealed class CreateRoleCommandHandler
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public CreateRoleCommandHandler(
        IApplicationDbContext applicationDbContext,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<RoleResponse> HandleAsync(
        CreateRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var code = command.Code.Trim();
        var name = command.Name.Trim();
        var description = string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim();

        await ValidateAsync(code, name, description, cancellationToken);

        var role = new Role
        {
            Code = code,
            Name = name,
            Description = description,
            IsActive = command.IsActive,
            CreatedAtUtc = _dateTimeProvider.UtcNow,
            CreatedBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? "system" : _currentUserService.UserId
        };

        await _applicationDbContext.AddRoleAsync(role, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return role.ToResponse();
    }

    private async Task ValidateAsync(
        string code,
        string name,
        string? description,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        AddRequiredAndLengthErrors(errors, "code", code, Role.CodeMaxLength);
        AddRequiredAndLengthErrors(errors, "name", name, Role.NameMaxLength);

        if (!string.IsNullOrWhiteSpace(description) && description.Length > Role.DescriptionMaxLength)
        {
            errors["description"] = [$"description must be {Role.DescriptionMaxLength} characters or fewer."];
        }

        if (!errors.Any() && await _applicationDbContext.RoleCodeExistsAsync(code, cancellationToken))
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
