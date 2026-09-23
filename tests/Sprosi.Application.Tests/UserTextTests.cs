using Sprosi.Application.Common;

namespace Sprosi.Application.Tests;

public sealed class UserTextTests
{
    [Fact]
    public void Russian_is_the_default_language()
    {
        var text = UserText.Get(null, ErrorCodes.PasswordTooShort);

        Assert.Equal("Пароль должен быть не короче 8 символов.", text);
    }

    [Fact]
    public void English_comes_from_the_first_accept_language_tag()
    {
        var text = UserText.Get("en-US,en;q=0.9,ru;q=0.8", ErrorCodes.PasswordTooShort);

        Assert.Equal("Password must be at least 8 characters.", text);
    }

    [Fact]
    public void Unknown_code_is_returned_as_is()
    {
        Assert.Equal("missing", UserText.Get("en", "missing"));
    }
}
