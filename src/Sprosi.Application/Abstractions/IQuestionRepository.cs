using Sprosi.Application.Questions;
using Sprosi.Domain;

namespace Sprosi.Application.Abstractions;

/// <summary>
/// Persistence for questions.
/// </summary>
public interface IQuestionRepository
{
    /// <summary>
    /// Loads a question together with its author and answers.
    /// </summary>
    /// <param name="id">Question identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The question, or null when it does not exist.</returns>
    Task<Question?> FindAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Inserts a question.
    /// </summary>
    /// <param name="question">Question to insert.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Question question, CancellationToken cancellationToken);

    /// <summary>
    /// Writes pending changes of a tracked question.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SaveAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a question and, through the database, its answers.
    /// </summary>
    /// <param name="question">Question to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveAsync(Question question, CancellationToken cancellationToken);

    /// <summary>
    /// Returns a page of questions for the public list.
    /// </summary>
    /// <param name="query">Filter, sort and page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Matching rows and the total count.</returns>
    Task<QuestionPage> ListAsync(QuestionListQuery query, CancellationToken cancellationToken);
}
