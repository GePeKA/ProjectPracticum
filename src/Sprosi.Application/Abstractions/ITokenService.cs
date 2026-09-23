using Sprosi.Domain;

namespace Sprosi.Application.Abstractions;

/// <summary>
/// Issues an access token for a signed-in account.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Creates a token for the account.
    /// </summary>
    /// <param name="user">Account that just registered or signed in.</param>
    /// <returns>Serialized token.</returns>
    string Create(User user);
}
