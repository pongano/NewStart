namespace CoreProject.Backend.Application.Common.Security;

public static class PermissionCodes
{
    public const string UsersManage = "users.manage";
    public const string RolesManage = "roles.manage";
    public const string PermissionsManage = "permissions.manage";
    public const string MenusManage = "menus.manage";
    public const string UserRolesManage = "user_roles.manage";
    public const string RolePermissionsManage = "role_permissions.manage";
    public const string MenuPermissionsManage = "menu_permissions.manage";
    public const string AccessGraphRead = "access_graph.read";
    public const string AuditLogsRead = "audit_logs.read";

    public static readonly IReadOnlyCollection<string> All =
    [
        UsersManage,
        RolesManage,
        PermissionsManage,
        MenusManage,
        UserRolesManage,
        RolePermissionsManage,
        MenuPermissionsManage,
        AccessGraphRead,
        AuditLogsRead
    ];
}
