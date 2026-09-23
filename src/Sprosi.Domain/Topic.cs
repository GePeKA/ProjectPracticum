namespace Sprosi.Domain;

/// <summary>
/// Subject area a question belongs to.
/// </summary>
public enum Topic
{
    /// <summary>Study and learning.</summary>
    Study = 1,

    /// <summary>Everyday life.</summary>
    Everyday = 2,

    /// <summary>The city and local life.</summary>
    City = 3,

    /// <summary>Technology.</summary>
    Tech = 4,

    /// <summary>Anything that does not fit the other topics.</summary>
    Other = 5,
}
