using Microsoft.EntityFrameworkCore;
using Sprosi.Domain;

namespace Sprosi.Data;

/// <summary>
/// EF Core session for the Sprosi database.
/// </summary>
public sealed class AppDbContext : DbContext
{
    /// <summary>
    /// Creates a context with the given options.
    /// </summary>
    /// <param name="options">Provider and connection options.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>Accounts.</summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>Questions.</summary>
    public DbSet<Question> Questions => Set<Question>();

    /// <summary>Answers.</summary>
    public DbSet<Answer> Answers => Set<Answer>();

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
