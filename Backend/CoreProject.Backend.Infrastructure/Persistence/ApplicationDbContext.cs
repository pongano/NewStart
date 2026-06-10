using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Domain.AccessControl.Entities;
using CoreProject.Backend.Domain.Audit.Entities;
using CoreProject.Backend.Domain.Configuration.Entities;
using CoreProject.Backend.Domain.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreProject.Backend.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ConfigurationEntry> ConfigurationEntries => Set<ConfigurationEntry>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<MenuPermission> MenuPermissions => Set<MenuPermission>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    IQueryable<UserAccount> IApplicationDbContext.UserAccounts => UserAccounts;
    IQueryable<RefreshToken> IApplicationDbContext.RefreshTokens => RefreshTokens;
    IQueryable<Role> IApplicationDbContext.Roles => Roles;
    IQueryable<Permission> IApplicationDbContext.Permissions => Permissions;
    IQueryable<Menu> IApplicationDbContext.Menus => Menus;
    IQueryable<UserRole> IApplicationDbContext.UserRoles => UserRoles;
    IQueryable<RolePermission> IApplicationDbContext.RolePermissions => RolePermissions;
    IQueryable<MenuPermission> IApplicationDbContext.MenuPermissions => MenuPermissions;
    IQueryable<AuditLog> IApplicationDbContext.AuditLogs => AuditLogs;

    public Task AddUserAccountAsync(UserAccount userAccount, CancellationToken cancellationToken = default)
    {
        return UserAccounts.AddAsync(userAccount, cancellationToken).AsTask();
    }

    public Task<bool> UserNameExistsAsync(string userName, CancellationToken cancellationToken = default)
    {
        return UserAccounts.AnyAsync(x => x.UserName == userName, cancellationToken);
    }

    public Task<bool> UserNameExistsAsync(string userName, Guid excludingId, CancellationToken cancellationToken = default)
    {
        return UserAccounts.AnyAsync(x => x.UserName == userName && x.Id != excludingId, cancellationToken);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return UserAccounts.AnyAsync(x => x.Email == email, cancellationToken);
    }

    public Task<bool> EmailExistsAsync(string email, Guid excludingId, CancellationToken cancellationToken = default)
    {
        return UserAccounts.AnyAsync(x => x.Email == email && x.Id != excludingId, cancellationToken);
    }

    public Task<UserAccount?> FindUserAccountByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return UserAccounts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<UserAccount>> ListUserAccountsAsync(CancellationToken cancellationToken = default)
    {
        return UserAccounts.ToListAsync(cancellationToken);
    }

    public Task RemoveUserAccountAsync(UserAccount userAccount, CancellationToken cancellationToken = default)
    {
        UserAccounts.Remove(userAccount);
        return Task.CompletedTask;
    }

    public Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        return RefreshTokens.AddAsync(refreshToken, cancellationToken).AsTask();
    }

    public Task<RefreshToken?> FindRefreshTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
    }

    public Task AddRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        return Roles.AddAsync(role, cancellationToken).AsTask();
    }

    public Task<bool> RoleCodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return Roles.AnyAsync(x => x.Code == code, cancellationToken);
    }

    public Task<bool> RoleCodeExistsAsync(string code, Guid excludingId, CancellationToken cancellationToken = default)
    {
        return Roles.AnyAsync(x => x.Code == code && x.Id != excludingId, cancellationToken);
    }

    public Task<Role?> FindRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Roles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<Role>> ListRolesAsync(CancellationToken cancellationToken = default)
    {
        return Roles
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }

    public Task RemoveRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        Roles.Remove(role);
        return Task.CompletedTask;
    }

    public Task AddPermissionAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        return Permissions.AddAsync(permission, cancellationToken).AsTask();
    }

    public Task<bool> PermissionCodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return Permissions.AnyAsync(x => x.Code == code, cancellationToken);
    }

    public Task<bool> PermissionCodeExistsAsync(string code, Guid excludingId, CancellationToken cancellationToken = default)
    {
        return Permissions.AnyAsync(x => x.Code == code && x.Id != excludingId, cancellationToken);
    }

    public Task<Permission?> FindPermissionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Permissions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<Permission>> ListPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return Permissions
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }

    public Task RemovePermissionAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        Permissions.Remove(permission);
        return Task.CompletedTask;
    }

    public Task AddMenuAsync(Menu menu, CancellationToken cancellationToken = default)
    {
        return Menus.AddAsync(menu, cancellationToken).AsTask();
    }

    public Task<bool> MenuCodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return Menus.AnyAsync(x => x.Code == code, cancellationToken);
    }

    public Task<bool> MenuCodeExistsAsync(string code, Guid excludingId, CancellationToken cancellationToken = default)
    {
        return Menus.AnyAsync(x => x.Code == code && x.Id != excludingId, cancellationToken);
    }

    public Task<Menu?> FindMenuByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Menus.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<Menu>> ListMenusAsync(CancellationToken cancellationToken = default)
    {
        return Menus
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> MenuHasChildrenAsync(Guid menuId, CancellationToken cancellationToken = default)
    {
        return Menus.AnyAsync(x => x.ParentId == menuId, cancellationToken);
    }

    public Task RemoveMenuAsync(Menu menu, CancellationToken cancellationToken = default)
    {
        Menus.Remove(menu);
        return Task.CompletedTask;
    }

    public Task AddUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default)
    {
        return UserRoles.AddAsync(userRole, cancellationToken).AsTask();
    }

    public Task<bool> UserRoleExistsAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return UserRoles.AnyAsync(x => x.UserId == userId && x.RoleId == roleId, cancellationToken);
    }

    public Task<List<UserRole>> ListUserRolesAsync(CancellationToken cancellationToken = default)
    {
        return UserRoles
            .OrderBy(x => x.UserId)
            .ThenBy(x => x.RoleId)
            .ToListAsync(cancellationToken);
    }

    public Task<UserRole?> FindUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return UserRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId, cancellationToken);
    }

    public Task RemoveUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default)
    {
        UserRoles.Remove(userRole);
        return Task.CompletedTask;
    }

    public Task AddRolePermissionAsync(RolePermission rolePermission, CancellationToken cancellationToken = default)
    {
        return RolePermissions.AddAsync(rolePermission, cancellationToken).AsTask();
    }

    public Task<bool> RolePermissionExistsAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        return RolePermissions.AnyAsync(x => x.RoleId == roleId && x.PermissionId == permissionId, cancellationToken);
    }

    public Task<List<RolePermission>> ListRolePermissionsAsync(CancellationToken cancellationToken = default)
    {
        return RolePermissions
            .OrderBy(x => x.RoleId)
            .ThenBy(x => x.PermissionId)
            .ToListAsync(cancellationToken);
    }

    public Task<RolePermission?> FindRolePermissionAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        return RolePermissions.FirstOrDefaultAsync(x => x.RoleId == roleId && x.PermissionId == permissionId, cancellationToken);
    }

    public Task RemoveRolePermissionAsync(RolePermission rolePermission, CancellationToken cancellationToken = default)
    {
        RolePermissions.Remove(rolePermission);
        return Task.CompletedTask;
    }

    public Task AddMenuPermissionAsync(MenuPermission menuPermission, CancellationToken cancellationToken = default)
    {
        return MenuPermissions.AddAsync(menuPermission, cancellationToken).AsTask();
    }

    public Task<bool> MenuPermissionExistsAsync(Guid menuId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        return MenuPermissions.AnyAsync(x => x.MenuId == menuId && x.PermissionId == permissionId, cancellationToken);
    }

    public Task<List<MenuPermission>> ListMenuPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return MenuPermissions
            .OrderBy(x => x.MenuId)
            .ThenBy(x => x.PermissionId)
            .ToListAsync(cancellationToken);
    }

    public Task<MenuPermission?> FindMenuPermissionAsync(Guid menuId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        return MenuPermissions.FirstOrDefaultAsync(x => x.MenuId == menuId && x.PermissionId == permissionId, cancellationToken);
    }

    public Task RemoveMenuPermissionAsync(MenuPermission menuPermission, CancellationToken cancellationToken = default)
    {
        MenuPermissions.Remove(menuPermission);
        return Task.CompletedTask;
    }

    public Task AddAuditLogAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        return AuditLogs.AddAsync(auditLog, cancellationToken).AsTask();
    }

    public Task<List<AuditLog>> ListAuditLogsAsync(int limit, CancellationToken cancellationToken = default)
    {
        return AuditLogs
            .OrderByDescending(x => x.OccurredAtUtc)
            .ThenByDescending(x => x.Id)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
