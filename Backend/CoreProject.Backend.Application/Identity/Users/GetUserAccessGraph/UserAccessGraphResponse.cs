namespace CoreProject.Backend.Application.Identity.Users.GetUserAccessGraph;

public sealed class UserAccessGraphResponse
{
    public Guid UserId { get; init; }

    public string UserName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public IReadOnlyCollection<UserAccessGraphRoleResponse> Roles { get; init; } = [];

    public IReadOnlyCollection<UserAccessGraphPermissionResponse> Permissions { get; init; } = [];

    public IReadOnlyCollection<UserAccessGraphMenuResponse> Menus { get; init; } = [];
}

public sealed class UserAccessGraphRoleResponse
{
    public Guid Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public bool IsActive { get; init; }
}

public sealed class UserAccessGraphPermissionResponse
{
    public Guid Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
}

public sealed class UserAccessGraphMenuResponse
{
    public Guid Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Route { get; init; }

    public string? Icon { get; init; }

    public int SortOrder { get; init; }

    public bool IsVisible { get; init; }

    public Guid? ParentId { get; init; }
}
