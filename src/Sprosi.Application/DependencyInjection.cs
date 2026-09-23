using Microsoft.Extensions.DependencyInjection;
using Sprosi.Application.Answers;
using Sprosi.Application.Auth;
using Sprosi.Application.Questions;

namespace Sprosi.Application;

/// <summary>
/// Registers application services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers use-case services.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<QuestionService>();
        services.AddScoped<AnswerService>();
        return services;
    }
}
