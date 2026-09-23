using Sprosi.Domain;

namespace Sprosi.Application.Questions;

/// <summary>
/// Which questions to include.
/// </summary>
public enum QuestionStatusFilter
{
    /// <summary>Every question.</summary>
    All,

    /// <summary>Questions without an accepted answer.</summary>
    Open,

    /// <summary>Questions with an accepted answer.</summary>
    Resolved,
}

/// <summary>
/// Order of the question list.
/// </summary>
public enum QuestionSort
{
    /// <summary>Newest first.</summary>
    New,

    /// <summary>Oldest first.</summary>
    Old,

    /// <summary>Most answers first.</summary>
    Popular,
}

/// <summary>
/// Filter for the public question list.
/// </summary>
/// <param name="Topic">Topic to keep, or null for every topic.</param>
/// <param name="Status">Whether an accepted answer is required.</param>
/// <param name="Sort">List order.</param>
/// <param name="Search">Title fragment, or null.</param>
/// <param name="Page">Page number, starting at 1.</param>
/// <param name="PageSize">Page size.</param>
public sealed record QuestionListQuery(
    Topic? Topic,
    QuestionStatusFilter Status,
    QuestionSort Sort,
    string? Search,
    int Page,
    int PageSize);

/// <summary>
/// One row of the question list.
/// </summary>
/// <param name="Id">Question identifier.</param>
/// <param name="Title">Title.</param>
/// <param name="Topic">Topic.</param>
/// <param name="AuthorDisplayName">Author name.</param>
/// <param name="CreatedAt">Creation time.</param>
/// <param name="AnswerCount">Number of answers.</param>
/// <param name="HasAcceptedAnswer">Whether a best answer is chosen.</param>
public sealed record QuestionListItem(
    Guid Id,
    string Title,
    Topic Topic,
    string AuthorDisplayName,
    DateTimeOffset CreatedAt,
    int AnswerCount,
    bool HasAcceptedAnswer);

/// <summary>
/// A page of questions.
/// </summary>
/// <param name="Items">Rows on this page.</param>
/// <param name="Page">Page number.</param>
/// <param name="PageSize">Page size.</param>
/// <param name="Total">Total rows matching the filter.</param>
public sealed record QuestionPage(
    IReadOnlyList<QuestionListItem> Items,
    int Page,
    int PageSize,
    int Total);

/// <summary>
/// An answer shown on the question page.
/// </summary>
/// <param name="Id">Answer identifier.</param>
/// <param name="Body">Text.</param>
/// <param name="AuthorDisplayName">Author name.</param>
/// <param name="IsAccepted">Whether this is the best answer.</param>
/// <param name="CreatedAt">Creation time.</param>
/// <param name="UpdatedAt">Last change.</param>
/// <param name="CanEdit">Whether the current user may change this answer.</param>
public sealed record AnswerItem(
    Guid Id,
    string Body,
    string AuthorDisplayName,
    bool IsAccepted,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    bool CanEdit);

/// <summary>
/// A question and its answers.
/// </summary>
/// <param name="Id">Question identifier.</param>
/// <param name="Title">Title.</param>
/// <param name="Body">Text.</param>
/// <param name="Topic">Topic.</param>
/// <param name="AuthorDisplayName">Author name.</param>
/// <param name="CreatedAt">Creation time.</param>
/// <param name="UpdatedAt">Last change.</param>
/// <param name="AnswerCount">Number of answers.</param>
/// <param name="HasAcceptedAnswer">Whether a best answer is chosen.</param>
/// <param name="CanEdit">Whether the current user may change the question.</param>
/// <param name="Answers">Answers, best one first.</param>
public sealed record QuestionDetails(
    Guid Id,
    string Title,
    string Body,
    Topic Topic,
    string AuthorDisplayName,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    int AnswerCount,
    bool HasAcceptedAnswer,
    bool CanEdit,
    IReadOnlyList<AnswerItem> Answers);

/// <summary>
/// Fields for a new question.
/// </summary>
/// <param name="Title">Title.</param>
/// <param name="Body">Text.</param>
/// <param name="Topic">Topic name.</param>
public sealed record CreateQuestionRequest(string? Title, string? Body, string? Topic);

/// <summary>
/// Fields that replace a question.
/// </summary>
/// <param name="Title">Title.</param>
/// <param name="Body">Text.</param>
/// <param name="Topic">Topic name.</param>
public sealed record UpdateQuestionRequest(string? Title, string? Body, string? Topic);
