namespace Sprosi.Application.Common;

/// <summary>
/// A failure of an application rule, safe to show to the caller.
/// </summary>
public abstract class AppException : Exception
{
    /// <summary>Stable code used to pick a language.</summary>
    public string Code { get; }

    /// <summary>
    /// Creates an exception. <see cref="Exception.Message"/> stays Russian for callers that do not send a language.
    /// </summary>
    /// <param name="code">Error code from <see cref="ErrorCodes"/>.</param>
    protected AppException(string code)
        : base(UserText.Get(null, code))
    {
        Code = code;
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
    /// <param name="code">Error code from <see cref="ErrorCodes"/>.</param>
    public ValidationException(string code)
        : base(code)
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
    /// <param name="code">Error code from <see cref="ErrorCodes"/>.</param>
    public UnauthorizedException(string code)
        : base(code)
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
    /// <param name="code">Error code from <see cref="ErrorCodes"/>.</param>
    public ForbiddenException(string code)
        : base(code)
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
    /// <param name="code">Error code from <see cref="ErrorCodes"/>.</param>
    public NotFoundException(string code)
        : base(code)
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
    /// <param name="code">Error code from <see cref="ErrorCodes"/>.</param>
    public ConflictException(string code)
        : base(code)
    {
    }
}
