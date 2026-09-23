using Microsoft.Extensions.DependencyInjection;
using Sprosi.Application.Abstractions;
using Sprosi.Infrastructure.Security;
using Sprosi.Infrastructure.Time;

namespace Sprosi.Infrastructure;

/// <summary>
/// Registers infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers password hashing, JWT issuing and the clock.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        return services;
    }
}
