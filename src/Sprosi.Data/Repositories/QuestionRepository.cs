using Microsoft.EntityFrameworkCore;
using Sprosi.Application.Abstractions;
using Sprosi.Domain;

namespace Sprosi.Data.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IQuestionRepository"/>.
/// </summary>
public sealed class QuestionRepository : IQuestionRepository
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Creates a repository for the current database session.
    /// </summary>
    /// <param name="db">Database context.</param>
    public QuestionRepository(AppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<Question?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        return _db.Questions
            .Include(question => question.Author)
            .Include(question => question.Answers)
            .ThenInclude(answer => answer.Author)
            .FirstOrDefaultAsync(question => question.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Question question, CancellationToken cancellationToken)
    {
        _db.Questions.Add(question);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task SaveAsync(CancellationToken cancellationToken)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(Question question, CancellationToken cancellationToken)
    {
        _db.Questions.Remove(question);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
