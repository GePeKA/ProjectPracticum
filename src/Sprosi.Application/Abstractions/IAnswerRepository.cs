using Sprosi.Domain;

namespace Sprosi.Application.Abstractions;

/// <summary>
/// Persistence for answers.
/// </summary>
public interface IAnswerRepository
{
    /// <summary>
    /// Loads an answer together with its author.
    /// </summary>
    /// <param name="id">Answer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The answer, or null when it does not exist.</returns>
    Task<Answer?> FindAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Inserts an answer.
    /// </summary>
    /// <param name="answer">Answer to insert.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Answer answer, CancellationToken cancellationToken);

    /// <summary>
    /// Writes pending changes of a tracked answer.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SaveAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an answer.
    /// </summary>
    /// <param name="answer">Answer to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveAsync(Answer answer, CancellationToken cancellationToken);
}
