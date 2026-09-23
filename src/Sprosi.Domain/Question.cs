namespace Sprosi.Domain;

/// <summary>
/// A public question other people can answer.
/// </summary>
public sealed class Question
{
    /// <summary>Stable identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Author of the question.</summary>
    public Guid AuthorId { get; set; }

    /// <summary>Author account.</summary>
    public User? Author { get; set; }

    /// <summary>Short title shown in the list.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Full text of the question.</summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>Topic used to filter the list.</summary>
    public Topic Topic { get; set; }

    /// <summary>Moment the question was created, in UTC.</summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Moment the question was last changed, in UTC.</summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>Answers written for this question.</summary>
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
