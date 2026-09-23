using Microsoft.AspNetCore.Identity;
using Sprosi.Application.Abstractions;

namespace Sprosi.Infrastructure.Security;

/// <summary>
/// Hashes passwords with the ASP.NET Core identity hasher.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _inner = new();

    /// <inheritdoc />
    public string Hash(string password) => _inner.HashPassword(new object(), password);

    /// <inheritdoc />
    public bool Verify(string password, string passwordHash) =>
        _inner.VerifyHashedPassword(new object(), passwordHash, password) != PasswordVerificationResult.Failed;
}
