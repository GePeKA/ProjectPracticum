namespace Sprosi.Application.Abstractions;

/// <summary>
/// Turns a password into a stored hash and checks it later.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Builds a hash that can be stored.
    /// </summary>
    /// <param name="password">Plain password.</param>
    /// <returns>Hash string.</returns>
    string Hash(string password);

    /// <summary>
    /// Checks a plain password against a stored hash.
    /// </summary>
    /// <param name="password">Plain password.</param>
    /// <param name="passwordHash">Hash stored on the account.</param>
    /// <returns>True when the password matches.</returns>
    bool Verify(string password, string passwordHash);
}
