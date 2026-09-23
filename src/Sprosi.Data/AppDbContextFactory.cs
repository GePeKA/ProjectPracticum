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
    /// <returns>A context pointed at the local or <c>SPROSI_CONNECTION</c> database.</returns>
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("SPROSI_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=sprosi;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
