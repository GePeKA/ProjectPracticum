using Sprosi.Application.Abstractions;
using Sprosi.Application.Common;
using Sprosi.Domain;

namespace Sprosi.Application.Questions;

/// <summary>
/// Lists questions and lets an author change their own.
/// </summary>
public sealed class QuestionService
{
    private readonly IQuestionRepository _questions;
    private readonly IClock _clock;

    /// <summary>
    /// Creates the service.
    /// </summary>
    /// <param name="questions">Question store.</param>
    /// <param name="clock">Current time.</param>
    public QuestionService(IQuestionRepository questions, IClock clock)
    {
        _questions = questions;
        _clock = clock;
    }

    /// <summary>
    /// Returns one page of public questions.
    /// </summary>
    /// <param name="topic">Topic name, or empty for every topic.</param>
    /// <param name="status">all, open or resolved.</param>
    /// <param name="sort">new, old or popular.</param>
    /// <param name="search">Title fragment.</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Page size. Values outside 1..50 are clamped.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The page.</returns>
    /// <exception cref="ValidationException">The topic, status or sort is unknown.</exception>
    public Task<QuestionPage> ListAsync(
        string? topic,
        string? status,
        string? sort,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new QuestionListQuery(
            ParseTopic(topic, required: false),
            ParseStatus(status),
            ParseSort(sort),
            string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
            page < 1 ? 1 : page,
            pageSize < 1 ? 20 : Math.Min(pageSize, 50));

        return _questions.ListAsync(query, cancellationToken);
    }

    /// <summary>
    /// Loads a question for a guest or a signed-in user.
    /// </summary>
    /// <param name="id">Question identifier.</param>
    /// <param name="currentUserId">Caller, or null for a guest.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The question and its answers.</returns>
    /// <exception cref="NotFoundException">The question does not exist.</exception>
    public async Task<QuestionDetails> GetAsync(Guid id, Guid? currentUserId, CancellationToken cancellationToken)
    {
        var question = await _questions.FindAsync(id, cancellationToken)
            ?? throw new NotFoundException("Вопрос не найден.");

        return ToDetails(question, currentUserId);
    }

    /// <summary>
    /// Creates a question for the signed-in user.
    /// </summary>
    /// <param name="authorId">Author identifier.</param>
    /// <param name="request">Title, text and topic.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created question.</returns>
    /// <exception cref="ValidationException">A field is missing or too long.</exception>
    public async Task<QuestionDetails> CreateAsync(
        Guid authorId,
        CreateQuestionRequest request,
        CancellationToken cancellationToken)
    {
        var (title, body, topic) = ReadFields(request.Title, request.Body, request.Topic);
        var now = _clock.UtcNow;
        var question = new Question
        {
            Id = Guid.NewGuid(),
            AuthorId = authorId,
            Title = title,
            Body = body,
            Topic = topic,
            CreatedAt = now,
            UpdatedAt = now,
        };

        await _questions.AddAsync(question, cancellationToken);
        return await GetAsync(question.Id, authorId, cancellationToken);
    }

    /// <summary>
    /// Replaces the title, text and topic of the author's question.
    /// </summary>
    /// <param name="id">Question identifier.</param>
    /// <param name="authorId">Caller identifier.</param>
    /// <param name="request">New fields.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated question.</returns>
    /// <exception cref="NotFoundException">The question does not exist.</exception>
    /// <exception cref="ForbiddenException">The caller is not the author.</exception>
    /// <exception cref="ValidationException">A field is missing or too long.</exception>
    public async Task<QuestionDetails> UpdateAsync(
        Guid id,
        Guid authorId,
        UpdateQuestionRequest request,
        CancellationToken cancellationToken)
    {
        var question = await _questions.FindAsync(id, cancellationToken)
            ?? throw new NotFoundException("Вопрос не найден.");

        if (question.AuthorId != authorId)
            throw new ForbiddenException("Нельзя изменить чужой вопрос.");

        var (title, body, topic) = ReadFields(request.Title, request.Body, request.Topic);
        question.Title = title;
        question.Body = body;
        question.Topic = topic;
        question.UpdatedAt = _clock.UtcNow;
        await _questions.SaveAsync(cancellationToken);
        return ToDetails(question, authorId);
    }

    /// <summary>
    /// Deletes the author's question.
    /// </summary>
    /// <param name="id">Question identifier.</param>
    /// <param name="authorId">Caller identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="NotFoundException">The question does not exist.</exception>
    /// <exception cref="ForbiddenException">The caller is not the author.</exception>
    public async Task DeleteAsync(Guid id, Guid authorId, CancellationToken cancellationToken)
    {
        var question = await _questions.FindAsync(id, cancellationToken)
            ?? throw new NotFoundException("Вопрос не найден.");

        if (question.AuthorId != authorId)
            throw new ForbiddenException("Нельзя удалить чужой вопрос.");

        await _questions.RemoveAsync(question, cancellationToken);
    }

    private static QuestionDetails ToDetails(Question question, Guid? currentUserId)
    {
        var answers = question.Answers
            .OrderByDescending(answer => answer.IsAccepted)
            .ThenBy(answer => answer.CreatedAt)
            .Select(answer => new AnswerItem(
                answer.Id,
                answer.Body,
                answer.Author?.DisplayName ?? string.Empty,
                answer.IsAccepted,
                answer.CreatedAt,
                answer.UpdatedAt,
                currentUserId == answer.AuthorId))
            .ToList();

        return new QuestionDetails(
            question.Id,
            question.Title,
            question.Body,
            question.Topic,
            question.Author?.DisplayName ?? string.Empty,
            question.CreatedAt,
            question.UpdatedAt,
            answers.Count,
            answers.Exists(answer => answer.IsAccepted),
            currentUserId == question.AuthorId,
            answers);
    }

    private static (string Title, string Body, Topic Topic) ReadFields(string? title, string? body, string? topic)
    {
        var normalizedTitle = title?.Trim() ?? string.Empty;
        var normalizedBody = body?.Trim() ?? string.Empty;
        if (normalizedTitle.Length == 0 || normalizedBody.Length == 0)
            throw new ValidationException("Заполните заголовок и текст вопроса.");

        if (normalizedTitle.Length > 120)
            throw new ValidationException("Заголовок не длиннее 120 символов.");

        if (normalizedBody.Length > 5000)
            throw new ValidationException("Текст вопроса не длиннее 5000 символов.");

        return (normalizedTitle, normalizedBody, ParseTopic(topic, required: true)!.Value);
    }

    private static Topic? ParseTopic(string? topic, bool required)
    {
        if (string.IsNullOrWhiteSpace(topic))
        {
            if (required)
                throw new ValidationException("Укажите тему.");
            return null;
        }

        if (!Enum.TryParse<Topic>(topic.Trim(), ignoreCase: true, out var parsed) || !Enum.IsDefined(parsed))
            throw new ValidationException("Неизвестная тема.");

        return parsed;
    }

    private static QuestionStatusFilter ParseStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status) || status.Equals("all", StringComparison.OrdinalIgnoreCase))
            return QuestionStatusFilter.All;

        if (status.Equals("open", StringComparison.OrdinalIgnoreCase))
            return QuestionStatusFilter.Open;

        if (status.Equals("resolved", StringComparison.OrdinalIgnoreCase))
            return QuestionStatusFilter.Resolved;

        throw new ValidationException("Неизвестное состояние.");
    }

    private static QuestionSort ParseSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort) || sort.Equals("new", StringComparison.OrdinalIgnoreCase))
            return QuestionSort.New;

        if (sort.Equals("old", StringComparison.OrdinalIgnoreCase))
            return QuestionSort.Old;

        if (sort.Equals("popular", StringComparison.OrdinalIgnoreCase))
            return QuestionSort.Popular;

        throw new ValidationException("Неизвестная сортировка.");
    }
}
