using Microsoft.EntityFrameworkCore;
using Sprosi.Application.Abstractions;
using Sprosi.Application.Questions;
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

    /// <inheritdoc />
    public async Task<QuestionPage> ListAsync(QuestionListQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Question> questions = _db.Questions.AsNoTracking();

        if (query.Topic is Topic topic)
            questions = questions.Where(question => question.Topic == topic);

        if (query.Status == QuestionStatusFilter.Open)
            questions = questions.Where(question => !question.Answers.Any(answer => answer.IsAccepted));

        if (query.Status == QuestionStatusFilter.Resolved)
            questions = questions.Where(question => question.Answers.Any(answer => answer.IsAccepted));

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var escaped = query.Search
                .Replace("\\", "\\\\", StringComparison.Ordinal)
                .Replace("%", "\\%", StringComparison.Ordinal)
                .Replace("_", "\\_", StringComparison.Ordinal);
            var pattern = $"%{escaped}%";
            questions = questions.Where(question => EF.Functions.ILike(question.Title, pattern, "\\"));
        }

        questions = query.Sort switch
        {
            QuestionSort.Old => questions.OrderBy(question => question.CreatedAt),
            QuestionSort.Popular => questions
                .OrderByDescending(question => question.Answers.Count)
                .ThenByDescending(question => question.CreatedAt),
            _ => questions.OrderByDescending(question => question.CreatedAt),
        };

        var total = await questions.CountAsync(cancellationToken);
        var items = await questions
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(question => new QuestionListItem(
                question.Id,
                question.Title,
                question.Topic,
                question.Author!.DisplayName,
                question.CreatedAt,
                question.Answers.Count,
                question.Answers.Any(answer => answer.IsAccepted)))
            .ToListAsync(cancellationToken);

        return new QuestionPage(items, query.Page, query.PageSize, total);
    }
}
