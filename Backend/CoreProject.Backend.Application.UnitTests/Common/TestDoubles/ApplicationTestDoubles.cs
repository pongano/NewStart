using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.AccessControl.Entities;
using CoreProject.Backend.Domain.Identity.Entities;

namespace CoreProject.Backend.Application.UnitTests.Common.TestDoubles;

internal sealed class FakeApplicationDbContext : IApplicationDbContext
{
    private readonly List<UserAccount> _userAccounts = [];
    private readonly List<Role> _roles = [];
    private readonly List<Permission> _permissions = [];
    private readonly List<Menu> _menus = [];
    private readonly List<UserRole> _userRoles = [];
    private readonly List<RolePermission> _rolePermissions = [];
    private readonly List<MenuPermission> _menuPermissions = [];

    public IQueryable<UserAccount> UserAccounts => _userAccounts.AsQueryable();
    public IQueryable<Role> Roles => _roles.AsQueryable();
    public IQueryable<Permission> Permissions => _permissions.AsQueryable();
    public IQueryable<Menu> Menus => _menus.AsQueryable();
    public IQueryable<UserRole> UserRoles => _userRoles.AsQueryable();
    public IQueryable<RolePermission> RolePermissions => _rolePermissions.AsQueryable();
    public IQueryable<MenuPermission> MenuPermissions => _menuPermissions.AsQueryable();

    public Task AddUserAccountAsync(UserAccount userAccount, CancellationToken cancellationToken = default)
    {
        _userAccounts.Add(userAccount);
        return Task.CompletedTask;
    }

    public Task<bool> UserNameExistsAsync(string userName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_userAccounts.Any(x => x.UserName == userName));
    }

    public Task<bool> UserNameExistsAsync(string userName, Guid excludingId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_userAccounts.Any(x => x.UserName == userName && x.Id != excludingId));
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_userAccounts.Any(x => x.Email == email));
    }

    public Task<bool> EmailExistsAsync(string email, Guid excludingId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_userAccounts.Any(x => x.Email == email && x.Id != excludingId));
    }

    public Task<UserAccount?> FindUserAccountByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_userAccounts.FirstOrDefault(x => x.Id == id));
    }

    public Task<List<UserAccount>> ListUserAccountsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_userAccounts.ToList());
    }

    public Task RemoveUserAccountAsync(UserAccount userAccount, CancellationToken cancellationToken = default)
    {
        _userAccounts.Remove(userAccount);
        return Task.CompletedTask;
    }

    public Task AddRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        _roles.Add(role);
        return Task.CompletedTask;
    }

    public Task<bool> RoleCodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_roles.Any(x => x.Code == code));
    }

    public Task<bool> RoleCodeExistsAsync(string code, Guid excludingId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_roles.Any(x => x.Code == code && x.Id != excludingId));
    }

    public Task<Role?> FindRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_roles.FirstOrDefault(x => x.Id == id));
    }

    public Task<List<Role>> ListRolesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_roles.OrderBy(x => x.Code, StringComparer.Ordinal).ToList());
    }

    public Task RemoveRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        _roles.Remove(role);
        return Task.CompletedTask;
    }

    public Task AddPermissionAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        _permissions.Add(permission);
        return Task.CompletedTask;
    }

    public Task<bool> PermissionCodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_permissions.Any(x => x.Code == code));
    }

    public Task<bool> PermissionCodeExistsAsync(string code, Guid excludingId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_permissions.Any(x => x.Code == code && x.Id != excludingId));
    }

    public Task<Permission?> FindPermissionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_permissions.FirstOrDefault(x => x.Id == id));
    }

    public Task<List<Permission>> ListPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_permissions.OrderBy(x => x.Code, StringComparer.Ordinal).ToList());
    }

    public Task RemovePermissionAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        _permissions.Remove(permission);
        return Task.CompletedTask;
    }

    public Task AddMenuAsync(Menu menu, CancellationToken cancellationToken = default)
    {
        _menus.Add(menu);
        return Task.CompletedTask;
    }

    public Task<bool> MenuCodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_menus.Any(x => x.Code == code));
    }

    public Task<bool> MenuCodeExistsAsync(string code, Guid excludingId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_menus.Any(x => x.Code == code && x.Id != excludingId));
    }

    public Task<Menu?> FindMenuByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_menus.FirstOrDefault(x => x.Id == id));
    }

    public Task<List<Menu>> ListMenusAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_menus.OrderBy(x => x.SortOrder).ThenBy(x => x.Code, StringComparer.Ordinal).ToList());
    }

    public Task<bool> MenuHasChildrenAsync(Guid menuId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_menus.Any(x => x.ParentId == menuId));
    }

    public Task RemoveMenuAsync(Menu menu, CancellationToken cancellationToken = default)
    {
        _menus.Remove(menu);
        return Task.CompletedTask;
    }

    public Task AddUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default)
    {
        _userRoles.Add(userRole);
        return Task.CompletedTask;
    }

    public Task<bool> UserRoleExistsAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_userRoles.Any(x => x.UserId == userId && x.RoleId == roleId));
    }

    public Task<List<UserRole>> ListUserRolesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_userRoles.OrderBy(x => x.UserId).ThenBy(x => x.RoleId).ToList());
    }

    public Task<UserRole?> FindUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_userRoles.FirstOrDefault(x => x.UserId == userId && x.RoleId == roleId));
    }

    public Task RemoveUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default)
    {
        _userRoles.Remove(userRole);
        return Task.CompletedTask;
    }

    public Task AddRolePermissionAsync(RolePermission rolePermission, CancellationToken cancellationToken = default)
    {
        _rolePermissions.Add(rolePermission);
        return Task.CompletedTask;
    }

    public Task<bool> RolePermissionExistsAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_rolePermissions.Any(x => x.RoleId == roleId && x.PermissionId == permissionId));
    }

    public Task<List<RolePermission>> ListRolePermissionsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_rolePermissions.OrderBy(x => x.RoleId).ThenBy(x => x.PermissionId).ToList());
    }

    public Task<RolePermission?> FindRolePermissionAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_rolePermissions.FirstOrDefault(x => x.RoleId == roleId && x.PermissionId == permissionId));
    }

    public Task RemoveRolePermissionAsync(RolePermission rolePermission, CancellationToken cancellationToken = default)
    {
        _rolePermissions.Remove(rolePermission);
        return Task.CompletedTask;
    }

    public Task AddMenuPermissionAsync(MenuPermission menuPermission, CancellationToken cancellationToken = default)
    {
        _menuPermissions.Add(menuPermission);
        return Task.CompletedTask;
    }

    public Task<bool> MenuPermissionExistsAsync(Guid menuId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_menuPermissions.Any(x => x.MenuId == menuId && x.PermissionId == permissionId));
    }

    public Task<List<MenuPermission>> ListMenuPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_menuPermissions.OrderBy(x => x.MenuId).ThenBy(x => x.PermissionId).ToList());
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(1);
    }

    public void Seed(params UserAccount[] userAccounts)
    {
        _userAccounts.AddRange(userAccounts);
    }

    public void Seed(params Role[] roles)
    {
        _roles.AddRange(roles);
    }

    public void Seed(params Permission[] permissions)
    {
        _permissions.AddRange(permissions);
    }

    public void Seed(params Menu[] menus)
    {
        _menus.AddRange(menus);
    }

    public void Seed(params UserRole[] userRoles)
    {
        _userRoles.AddRange(userRoles);
    }

    public void Seed(params RolePermission[] rolePermissions)
    {
        _rolePermissions.AddRange(rolePermissions);
    }

    public void Seed(params MenuPermission[] menuPermissions)
    {
        _menuPermissions.AddRange(menuPermissions);
    }
}

internal sealed class FakeDateTimeProvider : IDateTimeProvider
{
    public FakeDateTimeProvider(DateTime utcNow)
    {
        UtcNow = utcNow;
    }

    public DateTime UtcNow { get; }
}

internal sealed class FakeCurrentUserService : ICurrentUserService
{
    public FakeCurrentUserService(string? userId = null, bool isAuthenticated = false)
    {
        UserId = userId;
        IsAuthenticated = isAuthenticated;
    }

    public string? UserId { get; }

    public bool IsAuthenticated { get; }
}
