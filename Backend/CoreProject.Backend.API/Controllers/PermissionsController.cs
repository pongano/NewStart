using CoreProject.Backend.Application.AccessControl.Permissions;
using CoreProject.Backend.Application.AccessControl.Permissions.CreatePermission;
using CoreProject.Backend.Application.AccessControl.Permissions.DeletePermission;
using CoreProject.Backend.Application.AccessControl.Permissions.GetPermissionById;
using CoreProject.Backend.Application.AccessControl.Permissions.ListPermissions;
using CoreProject.Backend.Application.AccessControl.Permissions.UpdatePermission;
using Microsoft.AspNetCore.Mvc;

namespace CoreProject.Backend.API.Controllers;

[ApiController]
[Route("api/permissions")]
public sealed class PermissionsController : ApiControllerBase
{
    private readonly CreatePermissionCommandHandler _createPermissionCommandHandler;
    private readonly UpdatePermissionCommandHandler _updatePermissionCommandHandler;
    private readonly DeletePermissionCommandHandler _deletePermissionCommandHandler;
    private readonly GetPermissionByIdQueryHandler _getPermissionByIdQueryHandler;
    private readonly ListPermissionsQueryHandler _listPermissionsQueryHandler;

    public PermissionsController(
        CreatePermissionCommandHandler createPermissionCommandHandler,
        UpdatePermissionCommandHandler updatePermissionCommandHandler,
        DeletePermissionCommandHandler deletePermissionCommandHandler,
        GetPermissionByIdQueryHandler getPermissionByIdQueryHandler,
        ListPermissionsQueryHandler listPermissionsQueryHandler)
    {
        _createPermissionCommandHandler = createPermissionCommandHandler;
        _updatePermissionCommandHandler = updatePermissionCommandHandler;
        _deletePermissionCommandHandler = deletePermissionCommandHandler;
        _getPermissionByIdQueryHandler = getPermissionByIdQueryHandler;
        _listPermissionsQueryHandler = listPermissionsQueryHandler;
    }

    [HttpPost]
    [ProducesResponseType<PermissionResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PermissionResponse>> Create(
        [FromBody] CreatePermissionRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _createPermissionCommandHandler.HandleAsync(
            new CreatePermissionCommand
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description
            },
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<PermissionResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<PermissionResponse>>> List(CancellationToken cancellationToken)
    {
        var response = await _listPermissionsQueryHandler.HandleAsync(new ListPermissionsQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<PermissionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CoreProject.Backend.API.Common.Models.ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PermissionResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _getPermissionByIdQueryHandler.HandleAsync(new GetPermissionByIdQuery { Id = id }, cancellationToken);

        if (response is null)
        {
            return NotFoundError("Permission was not found.");
        }

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<PermissionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CoreProject.Backend.API.Common.Models.ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PermissionResponse>> Update(
        Guid id,
        [FromBody] UpdatePermissionRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _updatePermissionCommandHandler.HandleAsync(
            new UpdatePermissionCommand
            {
                Id = id,
                Code = request.Code,
                Name = request.Name,
                Description = request.Description
            },
            cancellationToken);

        if (response is null)
        {
            return NotFoundError("Permission was not found.");
        }

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CoreProject.Backend.API.Common.Models.ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _deletePermissionCommandHandler.HandleAsync(new DeletePermissionCommand { Id = id }, cancellationToken);

        if (!deleted)
        {
            return NotFoundError("Permission was not found.");
        }

        return NoContent();
    }

    public sealed class CreatePermissionRequest
    {
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    public sealed class UpdatePermissionRequest
    {
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
