namespace Sprosi.Application.Abstractions;

/// <summary>
/// Source of the current time.
/// </summary>
public interface IClock
{
    /// <summary>Current moment in UTC.</summary>
    DateTimeOffset UtcNow { get; }
}
