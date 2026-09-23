using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sprosi.Application.Abstractions;
using Sprosi.Data.Repositories;

namespace Sprosi.Data;

/// <summary>
/// Registers the data layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the database context and repositories.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>The same service collection.</returns>
    /// <exception cref="InvalidOperationException">Connection string Default is missing.</exception>
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Не задана строка подключения Default.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IAnswerRepository, AnswerRepository>();
        return services;
    }
}
