using Microsoft.AspNetCore.Mvc;
using Sprosi.Application.Auth;

namespace Sprosi.Api.Controllers;

/// <summary>
/// Registration and sign-in.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    /// <summary>
    /// Creates the controller.
    /// </summary>
    /// <param name="auth">Registration and sign-in service.</param>
    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    /// <summary>
    /// Creates an account.
    /// </summary>
    /// <param name="request">Email, password and display name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Account id, display name and access token.</returns>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResult>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _auth.RegisterAsync(request, cancellationToken));
    }

    /// <summary>
    /// Signs in with an existing account.
    /// </summary>
    /// <param name="request">Email and password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Account id, display name and access token.</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResult>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _auth.LoginAsync(request, cancellationToken));
    }
}
