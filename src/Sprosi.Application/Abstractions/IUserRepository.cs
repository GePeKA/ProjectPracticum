using Sprosi.Domain;

namespace Sprosi.Application.Abstractions;

/// <summary>
/// Persistence for accounts.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Finds an account by sign-in email.
    /// </summary>
    /// <param name="email">Email stored on the account.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The account, or null when it does not exist.</returns>
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Finds an account by identifier.
    /// </summary>
    /// <param name="id">Account identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The account, or null when it does not exist.</returns>
    Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Inserts a new account.
    /// </summary>
    /// <param name="user">Account to insert.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(User user, CancellationToken cancellationToken);
}
