using CoreProject.Backend.API.Security;
using CoreProject.Backend.Application.Common.Security;
using CoreProject.Backend.Application.Identity.Auth.BootstrapAdmin;
using CoreProject.Backend.Application.Identity.Auth.ChangePassword;
using CoreProject.Backend.Application.Identity.Auth.Login;
using CoreProject.Backend.Application.Identity.Auth.RefreshToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreProject.Backend.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly LoginCommandHandler _loginCommandHandler;
    private readonly BootstrapAdminCommandHandler _bootstrapAdminCommandHandler;
    private readonly RefreshTokenCommandHandler _refreshTokenCommandHandler;
    private readonly ChangePasswordCommandHandler _changePasswordCommandHandler;

    public AuthController(
        LoginCommandHandler loginCommandHandler,
        BootstrapAdminCommandHandler bootstrapAdminCommandHandler,
        RefreshTokenCommandHandler refreshTokenCommandHandler,
        ChangePasswordCommandHandler changePasswordCommandHandler)
    {
        _loginCommandHandler = loginCommandHandler;
        _bootstrapAdminCommandHandler = bootstrapAdminCommandHandler;
        _refreshTokenCommandHandler = refreshTokenCommandHandler;
        _changePasswordCommandHandler = changePasswordCommandHandler;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _loginCommandHandler.HandleAsync(
            new LoginCommand
            {
                Identifier = request.Identifier,
                Password = request.Password,
                IpAddress = GetClientIpAddress()
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponse>> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _refreshTokenCommandHandler.HandleAsync(
            new RefreshTokenCommand
            {
                RefreshToken = request.RefreshToken,
                IpAddress = GetClientIpAddress()
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("change-password")]
    [RequirePermission(PermissionCodes.UsersManage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var changed = await _changePasswordCommandHandler.HandleAsync(
            new ChangePasswordCommand
            {
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword
            },
            cancellationToken);

        return changed ? NoContent() : NotFound();
    }

    [HttpPost("bootstrap-admin")]
    [AllowAnonymous]
    [ProducesResponseType<BootstrapAdminResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BootstrapAdminResponse>> BootstrapAdmin(
        [FromBody] BootstrapAdminRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _bootstrapAdminCommandHandler.HandleAsync(
            new BootstrapAdminCommand
            {
            UserName = request.UserName,
                Email = request.Email,
                DisplayName = request.DisplayName,
                Password = request.Password
            },
            cancellationToken);

        return Created("/api/auth/login", response);
    }

    private string? GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    public sealed class LoginRequest
    {
        public string Identifier { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public sealed class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public sealed class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;
    }

    public sealed class BootstrapAdminRequest
    {
        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
