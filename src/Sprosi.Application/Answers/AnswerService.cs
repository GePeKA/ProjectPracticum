using Sprosi.Application.Abstractions;
using Sprosi.Application.Common;
using Sprosi.Application.Questions;
using Sprosi.Domain;

namespace Sprosi.Application.Answers;

/// <summary>
/// Writes answers and lets the question author choose the best one.
/// </summary>
public sealed class AnswerService
{
    private readonly IAnswerRepository _answers;
    private readonly IQuestionRepository _questions;
    private readonly QuestionService _questionService;
    private readonly IClock _clock;

    /// <summary>
    /// Creates the service.
    /// </summary>
    /// <param name="answers">Answer store.</param>
    /// <param name="questions">Question store.</param>
    /// <param name="questionService">Reads a question after it changes.</param>
    /// <param name="clock">Current time.</param>
    public AnswerService(
        IAnswerRepository answers,
        IQuestionRepository questions,
        QuestionService questionService,
        IClock clock)
    {
        _answers = answers;
        _questions = questions;
        _questionService = questionService;
        _clock = clock;
    }

    /// <summary>
    /// Adds an answer to a question.
    /// </summary>
    /// <param name="questionId">Question identifier.</param>
    /// <param name="authorId">Caller identifier.</param>
    /// <param name="body">Answer text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The question after the answer is saved.</returns>
    /// <exception cref="NotFoundException">The question does not exist.</exception>
    /// <exception cref="ValidationException">The text is empty or too long.</exception>
    public async Task<QuestionDetails> CreateAsync(
        Guid questionId,
        Guid authorId,
        string? body,
        CancellationToken cancellationToken)
    {
        _ = await _questions.FindAsync(questionId, cancellationToken)
            ?? throw new NotFoundException("Вопрос не найден.");

        var now = _clock.UtcNow;
        var answer = new Answer
        {
            Id = Guid.NewGuid(),
            QuestionId = questionId,
            AuthorId = authorId,
            Body = ReadBody(body),
            CreatedAt = now,
            UpdatedAt = now,
        };

        await _answers.AddAsync(answer, cancellationToken);
        return await _questionService.GetAsync(questionId, authorId, cancellationToken);
    }

    /// <summary>
    /// Replaces the text of the caller's answer.
    /// </summary>
    /// <param name="answerId">Answer identifier.</param>
    /// <param name="authorId">Caller identifier.</param>
    /// <param name="body">New text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The question after the change.</returns>
    /// <exception cref="NotFoundException">The answer does not exist.</exception>
    /// <exception cref="ForbiddenException">The caller is not the author.</exception>
    /// <exception cref="ValidationException">The text is empty or too long.</exception>
    public async Task<QuestionDetails> UpdateAsync(
        Guid answerId,
        Guid authorId,
        string? body,
        CancellationToken cancellationToken)
    {
        var answer = await _answers.FindAsync(answerId, cancellationToken)
            ?? throw new NotFoundException("Ответ не найден.");

        if (answer.AuthorId != authorId)
            throw new ForbiddenException("Нельзя изменить чужой ответ.");

        answer.Body = ReadBody(body);
        answer.UpdatedAt = _clock.UtcNow;
        await _answers.SaveAsync(cancellationToken);
        return await _questionService.GetAsync(answer.QuestionId, authorId, cancellationToken);
    }

    /// <summary>
    /// Deletes the caller's answer.
    /// </summary>
    /// <param name="answerId">Answer identifier.</param>
    /// <param name="authorId">Caller identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The question after the deletion.</returns>
    /// <exception cref="NotFoundException">The answer does not exist.</exception>
    /// <exception cref="ForbiddenException">The caller is not the author.</exception>
    public async Task<QuestionDetails> DeleteAsync(Guid answerId, Guid authorId, CancellationToken cancellationToken)
    {
        var answer = await _answers.FindAsync(answerId, cancellationToken)
            ?? throw new NotFoundException("Ответ не найден.");

        if (answer.AuthorId != authorId)
            throw new ForbiddenException("Нельзя удалить чужой ответ.");

        var questionId = answer.QuestionId;
        await _answers.RemoveAsync(answer, cancellationToken);
        return await _questionService.GetAsync(questionId, authorId, cancellationToken);
    }

    /// <summary>
    /// Marks another person's answer as the best one.
    /// </summary>
    /// <param name="answerId">Answer identifier.</param>
    /// <param name="userId">Question author.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The question with the new best answer.</returns>
    /// <exception cref="NotFoundException">The answer or question does not exist.</exception>
    /// <exception cref="ForbiddenException">The caller did not ask the question.</exception>
    /// <exception cref="ValidationException">The caller tried to accept their own answer.</exception>
    public async Task<QuestionDetails> AcceptAsync(Guid answerId, Guid userId, CancellationToken cancellationToken)
    {
        var question = await LoadQuestion(answerId, cancellationToken);
        var answer = question.Answers.First(item => item.Id == answerId);

        if (question.AuthorId != userId)
            throw new ForbiddenException("Отметить ответ может только автор вопроса.");

        if (answer.AuthorId == userId)
            throw new ValidationException("Нельзя отметить свой ответ как лучший.");

        foreach (var item in question.Answers)
            item.IsAccepted = item.Id == answer.Id;

        await _questions.SaveAsync(cancellationToken);
        return await _questionService.GetAsync(question.Id, userId, cancellationToken);
    }

    /// <summary>
    /// Clears the best-answer mark.
    /// </summary>
    /// <param name="answerId">Answer that currently holds the mark.</param>
    /// <param name="userId">Question author.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The question without a best answer.</returns>
    /// <exception cref="NotFoundException">The answer or question does not exist.</exception>
    /// <exception cref="ForbiddenException">The caller did not ask the question.</exception>
    /// <exception cref="ValidationException">This answer is not the best one.</exception>
    public async Task<QuestionDetails> ClearAcceptanceAsync(
        Guid answerId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var question = await LoadQuestion(answerId, cancellationToken);
        var answer = question.Answers.First(item => item.Id == answerId);

        if (question.AuthorId != userId)
            throw new ForbiddenException("Снять отметку может только автор вопроса.");

        if (!answer.IsAccepted)
            throw new ValidationException("Этот ответ не отмечен как лучший.");

        answer.IsAccepted = false;
        await _questions.SaveAsync(cancellationToken);
        return await _questionService.GetAsync(question.Id, userId, cancellationToken);
    }

    private async Task<Question> LoadQuestion(Guid answerId, CancellationToken cancellationToken)
    {
        var answer = await _answers.FindAsync(answerId, cancellationToken)
            ?? throw new NotFoundException("Ответ не найден.");

        return await _questions.FindAsync(answer.QuestionId, cancellationToken)
            ?? throw new NotFoundException("Вопрос не найден.");
    }

    private static string ReadBody(string? body)
    {
        var text = body?.Trim() ?? string.Empty;
        if (text.Length == 0)
            throw new ValidationException("Напишите текст ответа.");

        if (text.Length > 5000)
            throw new ValidationException("Текст ответа не длиннее 5000 символов.");

        return text;
    }
}
