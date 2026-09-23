namespace Sprosi.Application.Common;

/// <summary>
/// A failure of an application rule, safe to show to the caller.
/// </summary>
public abstract class AppException : Exception
{
    /// <summary>
    /// Creates an exception with a message for the client.
    /// </summary>
    /// <param name="message">Russian message returned by the API.</param>
    protected AppException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// The request is incomplete or breaks a field rule.
/// </summary>
public sealed class ValidationException : AppException
{
    /// <summary>
    /// Creates a validation error.
    /// </summary>
    /// <param name="message">What the caller should fix.</param>
    public ValidationException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// The caller is not signed in or the credentials are wrong.
/// </summary>
public sealed class UnauthorizedException : AppException
{
    /// <summary>
    /// Creates an authentication error.
    /// </summary>
    /// <param name="message">Message returned to the caller.</param>
    public UnauthorizedException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// The caller is signed in but cannot perform the action.
/// </summary>
public sealed class ForbiddenException : AppException
{
    /// <summary>
    /// Creates an authorization error.
    /// </summary>
    /// <param name="message">Message returned to the caller.</param>
    public ForbiddenException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// The requested record does not exist.
/// </summary>
public sealed class NotFoundException : AppException
{
    /// <summary>
    /// Creates a not-found error.
    /// </summary>
    /// <param name="message">Message returned to the caller.</param>
    public NotFoundException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// The request conflicts with data that is already stored.
/// </summary>
public sealed class ConflictException : AppException
{
    /// <summary>
    /// Creates a conflict error.
    /// </summary>
    /// <param name="message">Message returned to the caller.</param>
    public ConflictException(string message)
        : base(message)
    {
    }
}
