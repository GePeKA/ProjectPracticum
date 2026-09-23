using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprosi.Application.Answers;
using Sprosi.Application.Common;
using Sprosi.Application.Questions;

namespace Sprosi.Api.Controllers;

/// <summary>
/// Answers and the best-answer mark.
/// </summary>
[ApiController]
[Authorize]
public sealed class AnswersController : ControllerBase
{
    private readonly AnswerService _answers;

    /// <summary>
    /// Creates the controller.
    /// </summary>
    /// <param name="answers">Answer use cases.</param>
    public AnswersController(AnswerService answers)
    {
        _answers = answers;
    }

    /// <summary>
    /// Adds an answer to a question.
    /// </summary>
    /// <param name="questionId">Question identifier.</param>
    /// <param name="request">Answer text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The question with the new answer.</returns>
    [HttpPost("api/questions/{questionId:guid}/answers")]
    public Task<QuestionDetails> Create(
        Guid questionId,
        WriteAnswerRequest request,
        CancellationToken cancellationToken)
    {
        return _answers.CreateAsync(questionId, RequireUserId(), request.Body, cancellationToken);
    }

    /// <summary>
    /// Replaces the caller's answer.
    /// </summary>
    /// <param name="id">Answer identifier.</param>
    /// <param name="request">New text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The question after the change.</returns>
    [HttpPut("api/answers/{id:guid}")]
    public Task<QuestionDetails> Update(Guid id, WriteAnswerRequest request, CancellationToken cancellationToken)
    {
        return _answers.UpdateAsync(id, RequireUserId(), request.Body, cancellationToken);
    }

    /// <summary>
    /// Deletes the caller's answer.
    /// </summary>
    /// <param name="id">Answer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The question after the deletion.</returns>
    [HttpDelete("api/answers/{id:guid}")]
    public Task<QuestionDetails> Delete(Guid id, CancellationToken cancellationToken)
    {
        return _answers.DeleteAsync(id, RequireUserId(), cancellationToken);
    }

    /// <summary>
    /// Marks an answer as the best one.
    /// </summary>
    /// <param name="id">Answer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The question with the best answer selected.</returns>
    [HttpPost("api/answers/{id:guid}/accept")]
    public Task<QuestionDetails> Accept(Guid id, CancellationToken cancellationToken)
    {
        return _answers.AcceptAsync(id, RequireUserId(), cancellationToken);
    }

    /// <summary>
    /// Clears the best-answer mark.
    /// </summary>
    /// <param name="id">Answer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The question without a best answer.</returns>
    [HttpDelete("api/answers/{id:guid}/accept")]
    public Task<QuestionDetails> ClearAcceptance(Guid id, CancellationToken cancellationToken)
    {
        return _answers.ClearAcceptanceAsync(id, RequireUserId(), cancellationToken);
    }

    private Guid RequireUserId()
    {
        var value = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!Guid.TryParse(value, out var id))
            throw new UnauthorizedException(ErrorCodes.SignInRequired);

        return id;
    }
}

/// <summary>
/// Text of an answer.
/// </summary>
/// <param name="Body">Answer text.</param>
public sealed record WriteAnswerRequest(string? Body);
