using Sprosi.Application.Auth;
using Sprosi.Application.Common;
using Sprosi.Application.Tests.Fakes;

namespace Sprosi.Application.Tests;

public sealed class AuthServiceTests
{
    private readonly FakeUserRepository _users = new();
    private readonly FakePasswordHasher _passwords = new();
    private readonly FakeTokenService _tokens = new();
    private readonly FakeClock _clock = new();

    private AuthService Service => new(_users, _passwords, _tokens, _clock);

    [Fact]
    public async Task Register_stores_hash_and_returns_token()
    {
        var result = await Service.RegisterAsync(
            new RegisterRequest("Ada@Example.com", "password1", " Ада "),
            CancellationToken.None);

        var stored = Assert.Single(_users.Users);
        Assert.Equal("ada@example.com", stored.Email);
        Assert.Equal("Ада", stored.DisplayName);
        Assert.Equal("hashed:password1", stored.PasswordHash);
        Assert.Equal(_clock.UtcNow, stored.CreatedAt);
        Assert.Equal(stored.Id, result.UserId);
        Assert.Equal("token:" + stored.Id, result.Token);
    }

    [Fact]
    public async Task Register_rejects_short_password()
    {
        var error = await Assert.ThrowsAsync<ValidationException>(() =>
            Service.RegisterAsync(new RegisterRequest("ada@example.com", "short", "Ада"), CancellationToken.None));

        Assert.Equal("Пароль должен быть не короче 8 символов.", error.Message);
        Assert.Empty(_users.Users);
    }

    [Fact]
    public async Task Register_rejects_duplicate_email()
    {
        await Service.RegisterAsync(new RegisterRequest("ada@example.com", "password1", "Ада"), CancellationToken.None);

        var error = await Assert.ThrowsAsync<ConflictException>(() =>
            Service.RegisterAsync(new RegisterRequest("Ada@Example.com", "password2", "Другая"), CancellationToken.None));

        Assert.Equal("Пользователь с таким email уже зарегистрирован.", error.Message);
        Assert.Single(_users.Users);
    }

    [Fact]
    public async Task Login_rejects_wrong_password()
    {
        await Service.RegisterAsync(new RegisterRequest("ada@example.com", "password1", "Ада"), CancellationToken.None);

        var error = await Assert.ThrowsAsync<UnauthorizedException>(() =>
            Service.LoginAsync(new LoginRequest("ada@example.com", "password2"), CancellationToken.None));

        Assert.Equal("Неверный email или пароль.", error.Message);
    }

    [Fact]
    public async Task Login_returns_token_for_existing_user()
    {
        var registered = await Service.RegisterAsync(
            new RegisterRequest("ada@example.com", "password1", "Ада"),
            CancellationToken.None);

        var result = await Service.LoginAsync(
            new LoginRequest("ada@example.com", "password1"),
            CancellationToken.None);

        Assert.Equal(registered.UserId, result.UserId);
        Assert.Equal("Ада", result.DisplayName);
        Assert.Equal("token:" + registered.UserId, result.Token);
    }
}
