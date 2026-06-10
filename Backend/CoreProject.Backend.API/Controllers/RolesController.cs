using CoreProject.Backend.API.Security;
using CoreProject.Backend.Application.AccessControl.Menus;
using CoreProject.Backend.Application.AccessControl.RolePermissions;
using CoreProject.Backend.Application.AccessControl.RolePermissions.AssignPermissionToRole;
using CoreProject.Backend.Application.AccessControl.RolePermissions.ListPermissionsByRole;
using CoreProject.Backend.Application.AccessControl.RolePermissions.RemovePermissionFromRole;
using CoreProject.Backend.Application.AccessControl.RolePermissions.ReplaceRolePermissions;
using CoreProject.Backend.Application.AccessControl.Roles;
using CoreProject.Backend.Application.AccessControl.Roles.CreateRole;
using CoreProject.Backend.Application.AccessControl.Roles.DeleteRole;
using CoreProject.Backend.Application.AccessControl.Roles.GetRoleById;
using CoreProject.Backend.Application.AccessControl.Roles.ListMenusByRole;
using CoreProject.Backend.Application.AccessControl.Roles.ListRoles;
using CoreProject.Backend.Application.AccessControl.Roles.UpdateRole;
using CoreProject.Backend.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;

namespace CoreProject.Backend.API.Controllers;

[ApiController]
[Route("api/roles")]
[RequirePermission(PermissionCodes.RolesManage)]
public sealed class RolesController : ApiControllerBase
{
    private readonly CreateRoleCommandHandler _createRoleCommandHandler;
    private readonly UpdateRoleCommandHandler _updateRoleCommandHandler;
    private readonly DeleteRoleCommandHandler _deleteRoleCommandHandler;
    private readonly GetRoleByIdQueryHandler _getRoleByIdQueryHandler;
    private readonly ListRolesQueryHandler _listRolesQueryHandler;
    private readonly AssignPermissionToRoleCommandHandler _assignPermissionToRoleCommandHandler;
    private readonly ListPermissionsByRoleQueryHandler _listPermissionsByRoleQueryHandler;
    private readonly RemovePermissionFromRoleCommandHandler _removePermissionFromRoleCommandHandler;
    private readonly ReplaceRolePermissionsCommandHandler _replaceRolePermissionsCommandHandler;
    private readonly ListMenusByRoleQueryHandler _listMenusByRoleQueryHandler;

    public RolesController(
        CreateRoleCommandHandler createRoleCommandHandler,
        UpdateRoleCommandHandler updateRoleCommandHandler,
        DeleteRoleCommandHandler deleteRoleCommandHandler,
        GetRoleByIdQueryHandler getRoleByIdQueryHandler,
        ListRolesQueryHandler listRolesQueryHandler,
        AssignPermissionToRoleCommandHandler assignPermissionToRoleCommandHandler,
        ListPermissionsByRoleQueryHandler listPermissionsByRoleQueryHandler,
        RemovePermissionFromRoleCommandHandler removePermissionFromRoleCommandHandler,
        ReplaceRolePermissionsCommandHandler replaceRolePermissionsCommandHandler,
        ListMenusByRoleQueryHandler listMenusByRoleQueryHandler)
    {
        _createRoleCommandHandler = createRoleCommandHandler;
        _updateRoleCommandHandler = updateRoleCommandHandler;
        _deleteRoleCommandHandler = deleteRoleCommandHandler;
        _getRoleByIdQueryHandler = getRoleByIdQueryHandler;
        _listRolesQueryHandler = listRolesQueryHandler;
        _assignPermissionToRoleCommandHandler = assignPermissionToRoleCommandHandler;
        _listPermissionsByRoleQueryHandler = listPermissionsByRoleQueryHandler;
        _removePermissionFromRoleCommandHandler = removePermissionFromRoleCommandHandler;
        _replaceRolePermissionsCommandHandler = replaceRolePermissionsCommandHandler;
        _listMenusByRoleQueryHandler = listMenusByRoleQueryHandler;
    }

    [HttpPost]
    [ProducesResponseType<RoleResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoleResponse>> Create(
        [FromBody] CreateRoleRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _createRoleCommandHandler.HandleAsync(
            new CreateRoleCommand
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive ?? true
            },
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<RoleResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<RoleResponse>>> List(CancellationToken cancellationToken)
    {
        var response = await _listRolesQueryHandler.HandleAsync(new ListRolesQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<RoleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CoreProject.Backend.API.Common.Models.ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _getRoleByIdQueryHandler.HandleAsync(new GetRoleByIdQuery { Id = id }, cancellationToken);

        if (response is null)
        {
            return NotFoundError("Role was not found.");
        }

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<RoleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CoreProject.Backend.API.Common.Models.ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleResponse>> Update(
        Guid id,
        [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _updateRoleCommandHandler.HandleAsync(
            new UpdateRoleCommand
            {
                Id = id,
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive ?? true
            },
            cancellationToken);

        if (response is null)
        {
            return NotFoundError("Role was not found.");
        }

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CoreProject.Backend.API.Common.Models.ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _deleteRoleCommandHandler.HandleAsync(new DeleteRoleCommand { Id = id }, cancellationToken);

        if (!deleted)
        {
            return NotFoundError("Role was not found.");
        }

        return NoContent();
    }

    [HttpPost("{roleId:guid}/permissions/{permissionId:guid}")]
    [RequirePermission(PermissionCodes.RolePermissionsManage)]
    [ProducesResponseType<RolePermissionAssignmentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RolePermissionAssignmentResponse>> AssignPermission(
        Guid roleId,
        Guid permissionId,
        CancellationToken cancellationToken)
    {
        var response = await _assignPermissionToRoleCommandHandler.HandleAsync(
            new AssignPermissionToRoleCommand
            {
                RoleId = roleId,
                PermissionId = permissionId
            },
            cancellationToken);

        return CreatedAtAction(nameof(ListPermissions), new { roleId }, response);
    }

    [HttpGet("{roleId:guid}/permissions")]
    [RequirePermission(PermissionCodes.RolePermissionsManage)]
    [ProducesResponseType<IReadOnlyCollection<RolePermissionAssignmentResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<RolePermissionAssignmentResponse>>> ListPermissions(
        Guid roleId,
        CancellationToken cancellationToken)
    {
        var response = await _listPermissionsByRoleQueryHandler.HandleAsync(
            new ListPermissionsByRoleQuery { RoleId = roleId },
            cancellationToken);

        return Ok(response);
    }

    [HttpPut("{roleId:guid}/permissions")]
    [RequirePermission(PermissionCodes.RolePermissionsManage)]
    [ProducesResponseType<IReadOnlyCollection<RolePermissionAssignmentResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<RolePermissionAssignmentResponse>>> ReplacePermissions(
        Guid roleId,
        [FromBody] ReplaceRolePermissionsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _replaceRolePermissionsCommandHandler.HandleAsync(
            new ReplaceRolePermissionsCommand
            {
                RoleId = roleId,
                PermissionIds = request.PermissionIds
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpDelete("{roleId:guid}/permissions/{permissionId:guid}")]
    [RequirePermission(PermissionCodes.RolePermissionsManage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CoreProject.Backend.API.Common.Models.ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemovePermission(Guid roleId, Guid permissionId, CancellationToken cancellationToken)
    {
        var removed = await _removePermissionFromRoleCommandHandler.HandleAsync(
            new RemovePermissionFromRoleCommand
            {
                RoleId = roleId,
                PermissionId = permissionId
            },
            cancellationToken);

        if (!removed)
        {
            return NotFoundError("Role-permission assignment was not found.");
        }

        return NoContent();
    }

    [HttpGet("{roleId:guid}/menus")]
    [RequirePermission(PermissionCodes.AccessGraphRead)]
    [ProducesResponseType<IReadOnlyCollection<MenuResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<MenuResponse>>> ListMenus(
        Guid roleId,
        CancellationToken cancellationToken)
    {
        var response = await _listMenusByRoleQueryHandler.HandleAsync(
            new ListMenusByRoleQuery { RoleId = roleId },
            cancellationToken);

        return Ok(response);
    }

    public sealed class CreateRoleRequest
    {
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }

    public sealed class UpdateRoleRequest
    {
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }

    public sealed class ReplaceRolePermissionsRequest
    {
        public IReadOnlyCollection<Guid> PermissionIds { get; set; } = [];
    }
}
