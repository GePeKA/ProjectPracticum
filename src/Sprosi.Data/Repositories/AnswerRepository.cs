using Microsoft.EntityFrameworkCore;
using Sprosi.Application.Abstractions;
using Sprosi.Domain;

namespace Sprosi.Data.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IAnswerRepository"/>.
/// </summary>
public sealed class AnswerRepository : IAnswerRepository
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Creates a repository for the current database session.
    /// </summary>
    /// <param name="db">Database context.</param>
    public AnswerRepository(AppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<Answer?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        return _db.Answers
            .Include(answer => answer.Author)
            .FirstOrDefaultAsync(answer => answer.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Answer answer, CancellationToken cancellationToken)
    {
        _db.Answers.Add(answer);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task SaveAsync(CancellationToken cancellationToken)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(Answer answer, CancellationToken cancellationToken)
    {
        _db.Answers.Remove(answer);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
