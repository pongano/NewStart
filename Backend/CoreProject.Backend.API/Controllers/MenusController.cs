using CoreProject.Backend.API.Security;
using CoreProject.Backend.Application.AccessControl.Menus;
using CoreProject.Backend.Application.AccessControl.Menus.CreateMenu;
using CoreProject.Backend.Application.AccessControl.Menus.DeleteMenu;
using CoreProject.Backend.Application.AccessControl.Menus.GetMenuById;
using CoreProject.Backend.Application.AccessControl.Menus.ListMenus;
using CoreProject.Backend.Application.AccessControl.Menus.UpdateMenu;
using CoreProject.Backend.Application.AccessControl.MenuPermissions;
using CoreProject.Backend.Application.AccessControl.MenuPermissions.AssignPermissionToMenu;
using CoreProject.Backend.Application.AccessControl.MenuPermissions.ListPermissionsByMenu;
using CoreProject.Backend.Application.AccessControl.MenuPermissions.ReplaceMenuPermissions;
using CoreProject.Backend.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;

namespace CoreProject.Backend.API.Controllers;

[ApiController]
[Route("api/menus")]
[RequirePermission(PermissionCodes.MenusManage)]
public sealed class MenusController : ApiControllerBase
{
    private readonly CreateMenuCommandHandler _createMenuCommandHandler;
    private readonly UpdateMenuCommandHandler _updateMenuCommandHandler;
    private readonly DeleteMenuCommandHandler _deleteMenuCommandHandler;
    private readonly GetMenuByIdQueryHandler _getMenuByIdQueryHandler;
    private readonly ListMenusQueryHandler _listMenusQueryHandler;
    private readonly AssignPermissionToMenuCommandHandler _assignPermissionToMenuCommandHandler;
    private readonly ListPermissionsByMenuQueryHandler _listPermissionsByMenuQueryHandler;
    private readonly ReplaceMenuPermissionsCommandHandler _replaceMenuPermissionsCommandHandler;

    public MenusController(
        CreateMenuCommandHandler createMenuCommandHandler,
        UpdateMenuCommandHandler updateMenuCommandHandler,
        DeleteMenuCommandHandler deleteMenuCommandHandler,
        GetMenuByIdQueryHandler getMenuByIdQueryHandler,
        ListMenusQueryHandler listMenusQueryHandler,
        AssignPermissionToMenuCommandHandler assignPermissionToMenuCommandHandler,
        ListPermissionsByMenuQueryHandler listPermissionsByMenuQueryHandler,
        ReplaceMenuPermissionsCommandHandler replaceMenuPermissionsCommandHandler)
    {
        _createMenuCommandHandler = createMenuCommandHandler;
        _updateMenuCommandHandler = updateMenuCommandHandler;
        _deleteMenuCommandHandler = deleteMenuCommandHandler;
        _getMenuByIdQueryHandler = getMenuByIdQueryHandler;
        _listMenusQueryHandler = listMenusQueryHandler;
        _assignPermissionToMenuCommandHandler = assignPermissionToMenuCommandHandler;
        _listPermissionsByMenuQueryHandler = listPermissionsByMenuQueryHandler;
        _replaceMenuPermissionsCommandHandler = replaceMenuPermissionsCommandHandler;
    }

    [HttpPost]
    [ProducesResponseType<MenuResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MenuResponse>> Create(
        [FromBody] CreateMenuRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _createMenuCommandHandler.HandleAsync(
            new CreateMenuCommand
            {
                Code = request.Code,
                Name = request.Name,
                Route = request.Route,
                Icon = request.Icon,
                SortOrder = request.SortOrder,
                IsVisible = request.IsVisible ?? true,
                ParentId = request.ParentId
            },
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<MenuResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<MenuResponse>>> List(CancellationToken cancellationToken)
    {
        var response = await _listMenusQueryHandler.HandleAsync(new ListMenusQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<MenuResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CoreProject.Backend.API.Common.Models.ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MenuResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _getMenuByIdQueryHandler.HandleAsync(new GetMenuByIdQuery { Id = id }, cancellationToken);

        if (response is null)
        {
            return NotFoundError("Menu was not found.");
        }

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<MenuResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CoreProject.Backend.API.Common.Models.ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MenuResponse>> Update(
        Guid id,
        [FromBody] UpdateMenuRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _updateMenuCommandHandler.HandleAsync(
            new UpdateMenuCommand
            {
                Id = id,
                Code = request.Code,
                Name = request.Name,
                Route = request.Route,
                Icon = request.Icon,
                SortOrder = request.SortOrder,
                IsVisible = request.IsVisible ?? true,
                ParentId = request.ParentId
            },
            cancellationToken);

        if (response is null)
        {
            return NotFoundError("Menu was not found.");
        }

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CoreProject.Backend.API.Common.Models.ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _deleteMenuCommandHandler.HandleAsync(new DeleteMenuCommand { Id = id }, cancellationToken);

        if (!deleted)
        {
            return NotFoundError("Menu was not found.");
        }

        return NoContent();
    }

    [HttpPost("{menuId:guid}/permissions/{permissionId:guid}")]
    [RequirePermission(PermissionCodes.MenuPermissionsManage)]
    [ProducesResponseType<MenuPermissionResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MenuPermissionResponse>> AssignPermission(
        Guid menuId,
        Guid permissionId,
        CancellationToken cancellationToken)
    {
        var response = await _assignPermissionToMenuCommandHandler.HandleAsync(
            new AssignPermissionToMenuCommand
            {
                MenuId = menuId,
                PermissionId = permissionId
            },
            cancellationToken);

        return CreatedAtAction(nameof(ListPermissions), new { menuId }, response);
    }

    [HttpGet("{menuId:guid}/permissions")]
    [RequirePermission(PermissionCodes.MenuPermissionsManage)]
    [ProducesResponseType<IReadOnlyCollection<MenuPermissionResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<MenuPermissionResponse>>> ListPermissions(
        Guid menuId,
        CancellationToken cancellationToken)
    {
        var response = await _listPermissionsByMenuQueryHandler.HandleAsync(
            new ListPermissionsByMenuQuery { MenuId = menuId },
            cancellationToken);

        return Ok(response);
    }

    [HttpPut("{menuId:guid}/permissions")]
    [RequirePermission(PermissionCodes.MenuPermissionsManage)]
    [ProducesResponseType<IReadOnlyCollection<MenuPermissionResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<MenuPermissionResponse>>> ReplacePermissions(
        Guid menuId,
        [FromBody] ReplaceMenuPermissionsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _replaceMenuPermissionsCommandHandler.HandleAsync(
            new ReplaceMenuPermissionsCommand
            {
                MenuId = menuId,
                PermissionIds = request.PermissionIds
            },
            cancellationToken);

        return Ok(response);
    }

    public sealed class CreateMenuRequest
    {
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Route { get; set; }

        public string? Icon { get; set; }

        public int SortOrder { get; set; }

        public bool? IsVisible { get; set; }

        public Guid? ParentId { get; set; }
    }

    public sealed class UpdateMenuRequest
    {
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Route { get; set; }

        public string? Icon { get; set; }

        public int SortOrder { get; set; }

        public bool? IsVisible { get; set; }

        public Guid? ParentId { get; set; }
    }

    public sealed class ReplaceMenuPermissionsRequest
    {
        public IReadOnlyCollection<Guid> PermissionIds { get; set; } = [];
    }
}
