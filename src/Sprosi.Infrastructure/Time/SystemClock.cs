using Sprosi.Application.Abstractions;

namespace Sprosi.Infrastructure.Time;

/// <summary>
/// Clock based on the system UTC time.
/// </summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
