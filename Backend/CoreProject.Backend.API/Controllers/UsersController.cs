using CoreProject.Backend.Application.AccessControl.UserRoles;
using CoreProject.Backend.Application.AccessControl.UserRoles.AssignRoleToUser;
using CoreProject.Backend.Application.AccessControl.UserRoles.ListRolesByUser;
using CoreProject.Backend.Application.AccessControl.UserRoles.RemoveRoleFromUser;
using CoreProject.Backend.Application.Identity.Users;
using CoreProject.Backend.Application.Identity.Users.CreateUser;
using CoreProject.Backend.Application.Identity.Users.DeleteUser;
using CoreProject.Backend.Application.Identity.Users.GetUserAccessGraph;
using CoreProject.Backend.Application.Identity.Users.GetUserById;
using CoreProject.Backend.Application.Identity.Users.ListUsers;
using CoreProject.Backend.Application.Identity.Users.UpdateUser;
using Microsoft.AspNetCore.Mvc;

namespace CoreProject.Backend.API.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ApiControllerBase
{
    private readonly CreateUserCommandHandler _createUserCommandHandler;
    private readonly UpdateUserCommandHandler _updateUserCommandHandler;
    private readonly DeleteUserCommandHandler _deleteUserCommandHandler;
    private readonly GetUserByIdQueryHandler _getUserByIdQueryHandler;
    private readonly ListUsersQueryHandler _listUsersQueryHandler;
    private readonly AssignRoleToUserCommandHandler _assignRoleToUserCommandHandler;
    private readonly RemoveRoleFromUserCommandHandler _removeRoleFromUserCommandHandler;
    private readonly ListRolesByUserQueryHandler _listRolesByUserQueryHandler;
    private readonly GetUserAccessGraphQueryHandler _getUserAccessGraphQueryHandler;

    public UsersController(
        CreateUserCommandHandler createUserCommandHandler,
        UpdateUserCommandHandler updateUserCommandHandler,
        DeleteUserCommandHandler deleteUserCommandHandler,
        GetUserByIdQueryHandler getUserByIdQueryHandler,
        ListUsersQueryHandler listUsersQueryHandler,
        AssignRoleToUserCommandHandler assignRoleToUserCommandHandler,
        RemoveRoleFromUserCommandHandler removeRoleFromUserCommandHandler,
        ListRolesByUserQueryHandler listRolesByUserQueryHandler,
        GetUserAccessGraphQueryHandler getUserAccessGraphQueryHandler)
    {
        _createUserCommandHandler = createUserCommandHandler;
        _updateUserCommandHandler = updateUserCommandHandler;
        _deleteUserCommandHandler = deleteUserCommandHandler;
        _getUserByIdQueryHandler = getUserByIdQueryHandler;
        _listUsersQueryHandler = listUsersQueryHandler;
        _assignRoleToUserCommandHandler = assignRoleToUserCommandHandler;
        _removeRoleFromUserCommandHandler = removeRoleFromUserCommandHandler;
        _listRolesByUserQueryHandler = listRolesByUserQueryHandler;
        _getUserAccessGraphQueryHandler = getUserAccessGraphQueryHandler;
    }

    [HttpPost]
    [ProducesResponseType<UserAccountResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserAccountResponse>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _createUserCommandHandler.HandleAsync(
            new CreateUserCommand
            {
                UserName = request.UserName,
                Email = request.Email,
                DisplayName = request.DisplayName,
                IsActive = request.IsActive ?? true
            },
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<UserAccountResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<UserAccountResponse>>> List(CancellationToken cancellationToken)
    {
        var response = await _listUsersQueryHandler.HandleAsync(new ListUsersQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<UserAccountResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CoreProject.Backend.API.Common.Models.ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserAccountResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _getUserByIdQueryHandler.HandleAsync(new GetUserByIdQuery { Id = id }, cancellationToken);

        if (response is null)
        {
            return NotFoundError("User was not found.");
        }

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<UserAccountResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CoreProject.Backend.API.Common.Models.ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserAccountResponse>> Update(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _updateUserCommandHandler.HandleAsync(
            new UpdateUserCommand
            {
                Id = id,
                UserName = request.UserName,
                Email = request.Email,
                DisplayName = request.DisplayName,
                IsActive = request.IsActive ?? true
            },
            cancellationToken);

        if (response is null)
        {
            return NotFoundError("User was not found.");
        }

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CoreProject.Backend.API.Common.Models.ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _deleteUserCommandHandler.HandleAsync(new DeleteUserCommand { Id = id }, cancellationToken);

        if (!deleted)
        {
            return NotFoundError("User was not found.");
        }

        return NoContent();
    }

    [HttpPost("{userId:guid}/roles/{roleId:guid}")]
    [ProducesResponseType<UserRoleAssignmentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserRoleAssignmentResponse>> AssignRole(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken)
    {
        var response = await _assignRoleToUserCommandHandler.HandleAsync(
            new AssignRoleToUserCommand
            {
                UserId = userId,
                RoleId = roleId
            },
            cancellationToken);

        return CreatedAtAction(nameof(ListRoles), new { userId }, response);
    }

    [HttpGet("{userId:guid}/roles")]
    [ProducesResponseType<IReadOnlyCollection<UserRoleAssignmentResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<UserRoleAssignmentResponse>>> ListRoles(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await _listRolesByUserQueryHandler.HandleAsync(
            new ListRolesByUserQuery { UserId = userId },
            cancellationToken);

        return Ok(response);
    }

    [HttpDelete("{userId:guid}/roles/{roleId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CoreProject.Backend.API.Common.Models.ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRole(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        var removed = await _removeRoleFromUserCommandHandler.HandleAsync(
            new RemoveRoleFromUserCommand
            {
                UserId = userId,
                RoleId = roleId
            },
            cancellationToken);

        if (!removed)
        {
            return NotFoundError("User-role assignment was not found.");
        }

        return NoContent();
    }

    [HttpGet("{userId:guid}/access-graph")]
    [ProducesResponseType<UserAccessGraphResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserAccessGraphResponse>> GetAccessGraph(Guid userId, CancellationToken cancellationToken)
    {
        var response = await _getUserAccessGraphQueryHandler.HandleAsync(
            new GetUserAccessGraphQuery { UserId = userId },
            cancellationToken);

        return Ok(response);
    }

    public sealed class CreateUserRequest
    {
        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public bool? IsActive { get; set; }
    }

    public sealed class UpdateUserRequest
    {
        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public bool? IsActive { get; set; }
    }
}
