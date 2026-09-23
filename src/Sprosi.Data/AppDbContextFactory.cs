using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sprosi.Data;

/// <summary>
/// Builds a context for <c>dotnet ef</c> without starting the API.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <summary>
    /// Creates a context for migration commands.
    /// </summary>
    /// <param name="args">Arguments passed by the EF tool. Not used.</param>
    /// <returns>A context pointed at the local database from appsettings.</returns>
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Host=localhost;Port=5432;Database=sprosi;Username=postgres;Password=admin";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
