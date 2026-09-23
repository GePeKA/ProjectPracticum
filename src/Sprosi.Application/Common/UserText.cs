namespace Sprosi.Application.Common;

/// <summary>
/// Stable identifiers for messages shown to the user.
/// </summary>
public static class ErrorCodes
{
    /// <summary>Email, name or password is missing.</summary>
    public const string CredentialsRequired = "credentials_required";

    /// <summary>Email is not a valid address.</summary>
    public const string EmailInvalid = "email_invalid";

    /// <summary>Display name is longer than 50 characters.</summary>
    public const string DisplayNameTooLong = "display_name_too_long";

    /// <summary>Password is shorter than 8 characters.</summary>
    public const string PasswordTooShort = "password_too_short";

    /// <summary>Email is already registered.</summary>
    public const string EmailTaken = "email_taken";

    /// <summary>Email or password does not match.</summary>
    public const string InvalidCredentials = "invalid_credentials";

    /// <summary>The caller is not signed in.</summary>
    public const string SignInRequired = "sign_in_required";

    /// <summary>Question was not found.</summary>
    public const string QuestionNotFound = "question_not_found";

    /// <summary>Only the author may edit the question.</summary>
    public const string QuestionEditForbidden = "question_edit_forbidden";

    /// <summary>Only the author may delete the question.</summary>
    public const string QuestionDeleteForbidden = "question_delete_forbidden";

    /// <summary>Title or body is empty.</summary>
    public const string QuestionFieldsRequired = "question_fields_required";

    /// <summary>Title is longer than 120 characters.</summary>
    public const string TitleTooLong = "title_too_long";

    /// <summary>Question body is longer than 5000 characters.</summary>
    public const string QuestionBodyTooLong = "question_body_too_long";

    /// <summary>Topic was not provided.</summary>
    public const string TopicRequired = "topic_required";

    /// <summary>Topic name is unknown.</summary>
    public const string TopicUnknown = "topic_unknown";

    /// <summary>List status filter is unknown.</summary>
    public const string StatusUnknown = "status_unknown";

    /// <summary>List sort is unknown.</summary>
    public const string SortUnknown = "sort_unknown";

    /// <summary>Answer was not found.</summary>
    public const string AnswerNotFound = "answer_not_found";

    /// <summary>Only the author may edit the answer.</summary>
    public const string AnswerEditForbidden = "answer_edit_forbidden";

    /// <summary>Only the author may delete the answer.</summary>
    public const string AnswerDeleteForbidden = "answer_delete_forbidden";

    /// <summary>Only the question author may accept an answer.</summary>
    public const string AcceptForbidden = "accept_forbidden";

    /// <summary>The question author tried to accept their own answer.</summary>
    public const string AcceptOwnAnswer = "accept_own_answer";

    /// <summary>Only the question author may clear the mark.</summary>
    public const string ClearForbidden = "clear_forbidden";

    /// <summary>The answer is not the accepted one.</summary>
    public const string AnswerNotAccepted = "answer_not_accepted";

    /// <summary>Answer text is empty.</summary>
    public const string AnswerRequired = "answer_required";

    /// <summary>Answer text is longer than 5000 characters.</summary>
    public const string AnswerTooLong = "answer_too_long";
}

/// <summary>
/// Russian and English text for <see cref="ErrorCodes"/>.
/// </summary>
public static class UserText
{
    private static readonly Dictionary<string, (string Russian, string English)> Messages = new()
    {
        [ErrorCodes.CredentialsRequired] = ("Заполните email, имя и пароль.", "Fill in email, name and password."),
        [ErrorCodes.EmailInvalid] = ("Укажите корректный email.", "Enter a valid email."),
        [ErrorCodes.DisplayNameTooLong] = ("Имя не длиннее 50 символов.", "Name must be at most 50 characters."),
        [ErrorCodes.PasswordTooShort] = ("Пароль должен быть не короче 8 символов.", "Password must be at least 8 characters."),
        [ErrorCodes.EmailTaken] = ("Пользователь с таким email уже зарегистрирован.", "This email is already registered."),
        [ErrorCodes.InvalidCredentials] = ("Неверный email или пароль.", "Wrong email or password."),
        [ErrorCodes.SignInRequired] = ("Нужно войти.", "Sign in to continue."),
        [ErrorCodes.QuestionNotFound] = ("Вопрос не найден.", "Question was not found."),
        [ErrorCodes.QuestionEditForbidden] = ("Нельзя изменить чужой вопрос.", "You can only edit your own question."),
        [ErrorCodes.QuestionDeleteForbidden] = ("Нельзя удалить чужой вопрос.", "You can only delete your own question."),
        [ErrorCodes.QuestionFieldsRequired] = ("Заполните заголовок и текст вопроса.", "Fill in the title and the question text."),
        [ErrorCodes.TitleTooLong] = ("Заголовок не длиннее 120 символов.", "Title must be at most 120 characters."),
        [ErrorCodes.QuestionBodyTooLong] = ("Текст вопроса не длиннее 5000 символов.", "Question text must be at most 5000 characters."),
        [ErrorCodes.TopicRequired] = ("Укажите тему.", "Choose a topic."),
        [ErrorCodes.TopicUnknown] = ("Неизвестная тема.", "Unknown topic."),
        [ErrorCodes.StatusUnknown] = ("Неизвестное состояние.", "Unknown status."),
        [ErrorCodes.SortUnknown] = ("Неизвестная сортировка.", "Unknown sort."),
        [ErrorCodes.AnswerNotFound] = ("Ответ не найден.", "Answer was not found."),
        [ErrorCodes.AnswerEditForbidden] = ("Нельзя изменить чужой ответ.", "You can only edit your own answer."),
        [ErrorCodes.AnswerDeleteForbidden] = ("Нельзя удалить чужой ответ.", "You can only delete your own answer."),
        [ErrorCodes.AcceptForbidden] = ("Отметить ответ может только автор вопроса.", "Only the question author can choose the best answer."),
        [ErrorCodes.AcceptOwnAnswer] = ("Нельзя отметить свой ответ как лучший.", "You cannot mark your own answer as the best one."),
        [ErrorCodes.ClearForbidden] = ("Снять отметку может только автор вопроса.", "Only the question author can clear the mark."),
        [ErrorCodes.AnswerNotAccepted] = ("Этот ответ не отмечен как лучший.", "This answer is not marked as the best one."),
        [ErrorCodes.AnswerRequired] = ("Напишите текст ответа.", "Write the answer text."),
        [ErrorCodes.AnswerTooLong] = ("Текст ответа не длиннее 5000 символов.", "Answer text must be at most 5000 characters."),
    };

    /// <summary>
    /// Returns the message for a language tag from an Accept-Language header.
    /// </summary>
    /// <param name="acceptLanguage">Header value, or null.</param>
    /// <param name="code">Error code.</param>
    /// <returns>Localized text, or the code when it is unknown.</returns>
    public static string Get(string? acceptLanguage, string code)
    {
        if (!Messages.TryGetValue(code, out var text))
            return code;

        return PrefersEnglish(acceptLanguage) ? text.English : text.Russian;
    }

    private static bool PrefersEnglish(string? acceptLanguage)
    {
        if (string.IsNullOrWhiteSpace(acceptLanguage))
            return false;

        var first = acceptLanguage.Split(',')[0].Split(';')[0].Trim();
        return first.StartsWith("en", StringComparison.OrdinalIgnoreCase);
    }
}
