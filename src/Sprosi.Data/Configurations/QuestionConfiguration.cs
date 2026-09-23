using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sprosi.Domain;

namespace Sprosi.Data.Configurations;

/// <summary>
/// Maps <see cref="Question"/> to the questions table.
/// </summary>
public sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("questions");
        builder.HasKey(question => question.Id);
        builder.Property(question => question.Title).HasMaxLength(120).IsRequired();
        builder.Property(question => question.Body).HasMaxLength(5000).IsRequired();
        builder.Property(question => question.Topic).HasConversion<int>();

        builder.HasOne(question => question.Author)
            .WithMany()
            .HasForeignKey(question => question.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(question => question.Answers)
            .WithOne(answer => answer.Question)
            .HasForeignKey(answer => answer.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(question => question.CreatedAt);
        builder.HasIndex(question => question.Topic);
    }
}
