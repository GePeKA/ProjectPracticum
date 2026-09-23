namespace Sprosi.Domain;

/// <summary>
/// A public reply to a question.
/// </summary>
public sealed class Answer
{
    /// <summary>Stable identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Question this reply belongs to.</summary>
    public Guid QuestionId { get; set; }

    /// <summary>Question being answered.</summary>
    public Question? Question { get; set; }

    /// <summary>Author of the reply.</summary>
    public Guid AuthorId { get; set; }

    /// <summary>Author account.</summary>
    public User? Author { get; set; }

    /// <summary>Text of the reply.</summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>Whether the question author marked this reply as the one that helped.</summary>
    public bool IsAccepted { get; set; }

    /// <summary>Moment the reply was created, in UTC.</summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Moment the reply was last changed, in UTC.</summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
