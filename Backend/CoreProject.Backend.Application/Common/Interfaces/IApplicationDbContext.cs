using CoreProject.Backend.Domain.AccessControl.Entities;
using CoreProject.Backend.Domain.Identity.Entities;

namespace CoreProject.Backend.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    IQueryable<UserAccount> UserAccounts { get; }
    IQueryable<Role> Roles { get; }
    IQueryable<Permission> Permissions { get; }
    IQueryable<Menu> Menus { get; }
    IQueryable<UserRole> UserRoles { get; }
    IQueryable<RolePermission> RolePermissions { get; }
    IQueryable<MenuPermission> MenuPermissions { get; }

    Task AddUserAccountAsync(UserAccount userAccount, CancellationToken cancellationToken = default);

    Task<bool> UserNameExistsAsync(string userName, CancellationToken cancellationToken = default);

    Task<bool> UserNameExistsAsync(string userName, Guid excludingId, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, Guid excludingId, CancellationToken cancellationToken = default);

    Task<UserAccount?> FindUserAccountByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<UserAccount>> ListUserAccountsAsync(CancellationToken cancellationToken = default);

    Task RemoveUserAccountAsync(UserAccount userAccount, CancellationToken cancellationToken = default);

    Task AddRoleAsync(Role role, CancellationToken cancellationToken = default);

    Task<bool> RoleCodeExistsAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> RoleCodeExistsAsync(string code, Guid excludingId, CancellationToken cancellationToken = default);

    Task<Role?> FindRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<Role>> ListRolesAsync(CancellationToken cancellationToken = default);

    Task RemoveRoleAsync(Role role, CancellationToken cancellationToken = default);

    Task AddPermissionAsync(Permission permission, CancellationToken cancellationToken = default);

    Task<bool> PermissionCodeExistsAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> PermissionCodeExistsAsync(string code, Guid excludingId, CancellationToken cancellationToken = default);

    Task<Permission?> FindPermissionByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<Permission>> ListPermissionsAsync(CancellationToken cancellationToken = default);

    Task RemovePermissionAsync(Permission permission, CancellationToken cancellationToken = default);

    Task AddMenuAsync(Menu menu, CancellationToken cancellationToken = default);

    Task<bool> MenuCodeExistsAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> MenuCodeExistsAsync(string code, Guid excludingId, CancellationToken cancellationToken = default);

    Task<Menu?> FindMenuByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<Menu>> ListMenusAsync(CancellationToken cancellationToken = default);

    Task<bool> MenuHasChildrenAsync(Guid menuId, CancellationToken cancellationToken = default);

    Task RemoveMenuAsync(Menu menu, CancellationToken cancellationToken = default);

    Task AddUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default);

    Task<bool> UserRoleExistsAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    Task<List<UserRole>> ListUserRolesAsync(CancellationToken cancellationToken = default);

    Task<UserRole?> FindUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    Task RemoveUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default);

    Task AddRolePermissionAsync(RolePermission rolePermission, CancellationToken cancellationToken = default);

    Task<bool> RolePermissionExistsAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);

    Task<List<RolePermission>> ListRolePermissionsAsync(CancellationToken cancellationToken = default);

    Task<RolePermission?> FindRolePermissionAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);

    Task RemoveRolePermissionAsync(RolePermission rolePermission, CancellationToken cancellationToken = default);

    Task AddMenuPermissionAsync(MenuPermission menuPermission, CancellationToken cancellationToken = default);

    Task<bool> MenuPermissionExistsAsync(Guid menuId, Guid permissionId, CancellationToken cancellationToken = default);

    Task<List<MenuPermission>> ListMenuPermissionsAsync(CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
