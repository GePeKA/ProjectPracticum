using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprosi.Application.Common;
using Sprosi.Application.Questions;

namespace Sprosi.Api.Controllers;

/// <summary>
/// Public questions and the author's changes.
/// </summary>
[ApiController]
[Route("api/questions")]
public sealed class QuestionsController : ControllerBase
{
    private readonly QuestionService _questions;

    /// <summary>
    /// Creates the controller.
    /// </summary>
    /// <param name="questions">Question use cases.</param>
    public QuestionsController(QuestionService questions)
    {
        _questions = questions;
    }

    /// <summary>
    /// Returns a page of questions.
    /// </summary>
    /// <param name="topic">Topic name, or empty.</param>
    /// <param name="status">all, open or resolved.</param>
    /// <param name="sort">new, old or popular.</param>
    /// <param name="q">Title fragment.</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The page.</returns>
    [HttpGet]
    public Task<QuestionPage> List(
        [FromQuery] string? topic,
        [FromQuery] string? status,
        [FromQuery] string? sort,
        [FromQuery] string? q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return _questions.ListAsync(topic, status, sort, q, page, pageSize, cancellationToken);
    }

    /// <summary>
    /// Returns one question and its answers.
    /// </summary>
    /// <param name="id">Question identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The question.</returns>
    [HttpGet("{id:guid}")]
    public Task<QuestionDetails> Get(Guid id, CancellationToken cancellationToken)
    {
        return _questions.GetAsync(id, CurrentUserId(), cancellationToken);
    }

    /// <summary>
    /// Creates a question for the signed-in user.
    /// </summary>
    /// <param name="request">Title, text and topic.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created question.</returns>
    [Authorize]
    [HttpPost]
    public Task<QuestionDetails> Create(CreateQuestionRequest request, CancellationToken cancellationToken)
    {
        return _questions.CreateAsync(RequireUserId(), request, cancellationToken);
    }

    /// <summary>
    /// Replaces the author's question.
    /// </summary>
    /// <param name="id">Question identifier.</param>
    /// <param name="request">New fields.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated question.</returns>
    [Authorize]
    [HttpPut("{id:guid}")]
    public Task<QuestionDetails> Update(Guid id, UpdateQuestionRequest request, CancellationToken cancellationToken)
    {
        return _questions.UpdateAsync(id, RequireUserId(), request, cancellationToken);
    }

    /// <summary>
    /// Deletes the author's question.
    /// </summary>
    /// <param name="id">Question identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _questions.DeleteAsync(id, RequireUserId(), cancellationToken);
        return NoContent();
    }

    private Guid RequireUserId()
    {
        var id = CurrentUserId();
        if (id is null)
            throw new UnauthorizedException("Нужно войти.");

        return id.Value;
    }

    private Guid? CurrentUserId()
    {
        var value = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
