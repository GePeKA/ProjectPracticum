namespace Sprosi.Domain;

/// <summary>
/// A person who asks questions and writes answers.
/// </summary>
public sealed class User
{
    /// <summary>Stable identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Unique sign-in address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Name shown next to questions and answers.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Password hash. The plain password is never stored.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Moment the account was created, in UTC.</summary>
    public DateTime CreatedAt { get; set; }
}
