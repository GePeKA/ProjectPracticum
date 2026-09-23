namespace Sprosi.Application.Auth;

/// <summary>
/// Data for a new account.
/// </summary>
/// <param name="Email">Sign-in address.</param>
/// <param name="Password">Plain password.</param>
/// <param name="DisplayName">Name shown next to posts.</param>
public sealed record RegisterRequest(string? Email, string? Password, string? DisplayName);

/// <summary>
/// Credentials for an existing account.
/// </summary>
/// <param name="Email">Sign-in address.</param>
/// <param name="Password">Plain password.</param>
public sealed record LoginRequest(string? Email, string? Password);

/// <summary>
/// Result of a successful registration or sign-in.
/// </summary>
/// <param name="UserId">Account identifier.</param>
/// <param name="DisplayName">Name shown next to posts.</param>
/// <param name="Token">Access token.</param>
public sealed record AuthResult(Guid UserId, string DisplayName, string Token);
