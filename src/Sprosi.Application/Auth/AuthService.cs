using Sprosi.Application.Abstractions;
using Sprosi.Application.Common;
using Sprosi.Domain;

namespace Sprosi.Application.Auth;

/// <summary>
/// Registers accounts and checks credentials.
/// </summary>
public sealed class AuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwords;
    private readonly ITokenService _tokens;
    private readonly IClock _clock;

    /// <summary>
    /// Creates the service.
    /// </summary>
    /// <param name="users">Account store.</param>
    /// <param name="passwords">Password hashing.</param>
    /// <param name="tokens">Access tokens.</param>
    /// <param name="clock">Current time.</param>
    public AuthService(IUserRepository users, IPasswordHasher passwords, ITokenService tokens, IClock clock)
    {
        _users = users;
        _passwords = passwords;
        _tokens = tokens;
        _clock = clock;
    }

    /// <summary>
    /// Creates an account and returns a token.
    /// </summary>
    /// <param name="request">Email, password and display name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The new account and a token.</returns>
    /// <exception cref="ValidationException">A field is missing or too short.</exception>
    /// <exception cref="ConflictException">The email is already registered.</exception>
    public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var email = NormalizeEmail(request.Email);
        var displayName = request.DisplayName?.Trim() ?? string.Empty;
        var password = request.Password ?? string.Empty;

        if (email.Length == 0 || displayName.Length == 0 || password.Length == 0)
            throw new ValidationException("Заполните email, имя и пароль.");

        if (!IsEmail(email))
            throw new ValidationException("Укажите корректный email.");

        if (displayName.Length > 50)
            throw new ValidationException("Имя не длиннее 50 символов.");

        if (password.Length < 8)
            throw new ValidationException("Пароль должен быть не короче 8 символов.");

        if (await _users.FindByEmailAsync(email, cancellationToken) is not null)
            throw new ConflictException("Пользователь с таким email уже зарегистрирован.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            DisplayName = displayName,
            PasswordHash = _passwords.Hash(password),
            CreatedAt = _clock.UtcNow,
        };

        await _users.AddAsync(user, cancellationToken);
        return new AuthResult(user.Id, user.DisplayName, _tokens.Create(user));
    }

    /// <summary>
    /// Checks credentials and returns a token.
    /// </summary>
    /// <param name="request">Email and password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The account and a token.</returns>
    /// <exception cref="UnauthorizedException">The email or password does not match.</exception>
    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var email = NormalizeEmail(request.Email);
        var password = request.Password ?? string.Empty;
        var user = email.Length == 0
            ? null
            : await _users.FindByEmailAsync(email, cancellationToken);

        if (user is null || !_passwords.Verify(password, user.PasswordHash))
            throw new UnauthorizedException("Неверный email или пароль.");

        return new AuthResult(user.Id, user.DisplayName, _tokens.Create(user));
    }

    private static string NormalizeEmail(string? email) =>
        email?.Trim().ToLowerInvariant() ?? string.Empty;

    private static bool IsEmail(string email)
    {
        var at = email.IndexOf('@');
        return at > 0
            && at < email.Length - 3
            && email.IndexOf('.', at + 1) > at + 1
            && !email.Contains(' ');
    }
}
