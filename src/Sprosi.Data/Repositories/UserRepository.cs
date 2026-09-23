using Microsoft.EntityFrameworkCore;
using Sprosi.Application.Abstractions;
using Sprosi.Domain;

namespace Sprosi.Data.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IUserRepository"/>.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Creates a repository for the current database session.
    /// </summary>
    /// <param name="db">Database context.</param>
    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _db.Users.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    /// <inheritdoc />
    public Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _db.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
