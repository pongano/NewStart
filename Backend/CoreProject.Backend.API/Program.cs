using CoreProject.Backend.API.Common.Services;
using CoreProject.Backend.API.Middleware;
using CoreProject.Backend.Application.AccessControl;
using CoreProject.Backend.Application.AccessControl.Menus.CreateMenu;
using CoreProject.Backend.Application.AccessControl.Menus.DeleteMenu;
using CoreProject.Backend.Application.AccessControl.Menus.GetMenuById;
using CoreProject.Backend.Application.AccessControl.Menus.ListMenus;
using CoreProject.Backend.Application.AccessControl.Menus.UpdateMenu;
using CoreProject.Backend.Application.AccessControl.MenuPermissions.AssignPermissionToMenu;
using CoreProject.Backend.Application.AccessControl.MenuPermissions.ListPermissionsByMenu;
using CoreProject.Backend.Application.AccessControl.Permissions.CreatePermission;
using CoreProject.Backend.Application.AccessControl.Permissions.DeletePermission;
using CoreProject.Backend.Application.AccessControl.Permissions.GetPermissionById;
using CoreProject.Backend.Application.AccessControl.Permissions.ListPermissions;
using CoreProject.Backend.Application.AccessControl.Permissions.UpdatePermission;
using CoreProject.Backend.Application.AccessControl.RolePermissions.AssignPermissionToRole;
using CoreProject.Backend.Application.AccessControl.RolePermissions.ListPermissionsByRole;
using CoreProject.Backend.Application.AccessControl.RolePermissions.RemovePermissionFromRole;
using CoreProject.Backend.Application.AccessControl.Roles.CreateRole;
using CoreProject.Backend.Application.AccessControl.Roles.DeleteRole;
using CoreProject.Backend.Application.AccessControl.Roles.GetRoleById;
using CoreProject.Backend.Application.AccessControl.Roles.ListRoles;
using CoreProject.Backend.Application.AccessControl.Roles.UpdateRole;
using CoreProject.Backend.Application.AccessControl.UserRoles.AssignRoleToUser;
using CoreProject.Backend.Application.AccessControl.UserRoles.ListRolesByUser;
using CoreProject.Backend.Application.AccessControl.UserRoles.RemoveRoleFromUser;
using CoreProject.Backend.Application.Common.Interfaces;
using CoreProject.Backend.Application.Identity;
using CoreProject.Backend.Application.Identity.Users.CreateUser;
using CoreProject.Backend.Application.Identity.Users.DeleteUser;
using CoreProject.Backend.Application.Identity.Users.GetUserAccessGraph;
using CoreProject.Backend.Application.Identity.Users.GetUserById;
using CoreProject.Backend.Application.Identity.Users.ListUsers;
using CoreProject.Backend.Application.Identity.Users.UpdateUser;
using CoreProject.Backend.Application.SystemInfo;
using CoreProject.Backend.Infrastructure;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod
        | HttpLoggingFields.RequestPath
        | HttpLoggingFields.ResponseStatusCode
        | HttpLoggingFields.Duration;
});
builder.Services.AddHealthChecks();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<GetIdentityModuleSummaryQueryHandler>();
builder.Services.AddScoped<GetAccessControlModuleSummaryQueryHandler>();
builder.Services.AddScoped<CreateRoleCommandHandler>();
builder.Services.AddScoped<UpdateRoleCommandHandler>();
builder.Services.AddScoped<DeleteRoleCommandHandler>();
builder.Services.AddScoped<GetRoleByIdQueryHandler>();
builder.Services.AddScoped<ListRolesQueryHandler>();
builder.Services.AddScoped<CreatePermissionCommandHandler>();
builder.Services.AddScoped<UpdatePermissionCommandHandler>();
builder.Services.AddScoped<DeletePermissionCommandHandler>();
builder.Services.AddScoped<GetPermissionByIdQueryHandler>();
builder.Services.AddScoped<ListPermissionsQueryHandler>();
builder.Services.AddScoped<CreateMenuCommandHandler>();
builder.Services.AddScoped<UpdateMenuCommandHandler>();
builder.Services.AddScoped<DeleteMenuCommandHandler>();
builder.Services.AddScoped<GetMenuByIdQueryHandler>();
builder.Services.AddScoped<ListMenusQueryHandler>();
builder.Services.AddScoped<AssignPermissionToMenuCommandHandler>();
builder.Services.AddScoped<ListPermissionsByMenuQueryHandler>();
builder.Services.AddScoped<AssignPermissionToRoleCommandHandler>();
builder.Services.AddScoped<ListPermissionsByRoleQueryHandler>();
builder.Services.AddScoped<RemovePermissionFromRoleCommandHandler>();
builder.Services.AddScoped<AssignRoleToUserCommandHandler>();
builder.Services.AddScoped<ListRolesByUserQueryHandler>();
builder.Services.AddScoped<RemoveRoleFromUserCommandHandler>();
builder.Services.AddScoped<CreateUserCommandHandler>();
builder.Services.AddScoped<UpdateUserCommandHandler>();
builder.Services.AddScoped<DeleteUserCommandHandler>();
builder.Services.AddScoped<GetUserByIdQueryHandler>();
builder.Services.AddScoped<ListUsersQueryHandler>();
builder.Services.AddScoped<GetUserAccessGraphQueryHandler>();
builder.Services.AddScoped<GetSystemInfoQueryHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpLogging();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

public partial class Program
{
}
