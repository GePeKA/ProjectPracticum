using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sprosi.Application.Abstractions;
using Sprosi.Domain;

namespace Sprosi.Infrastructure.Security;

/// <summary>
/// Issues a signed JWT that lives for 24 hours.
/// </summary>
public sealed class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Creates the service.
    /// </summary>
    /// <param name="configuration">Application configuration with the Jwt section.</param>
    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public string Create(User user)
    {
        var section = _configuration.GetSection("Jwt");
        var key = section["Key"] ?? string.Empty;
        if (key.Length < 32)
            throw new InvalidOperationException("Ключ JWT должен быть не короче 32 символов.");

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: section["Issuer"],
            audience: section["Audience"],
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.DisplayName),
            ],
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
