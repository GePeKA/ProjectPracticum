using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sprosi.Domain;

namespace Sprosi.Data.Configurations;

/// <summary>
/// Maps <see cref="Answer"/> to the answers table.
/// </summary>
public sealed class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("answers");
        builder.HasKey(answer => answer.Id);
        builder.Property(answer => answer.Body).HasMaxLength(5000).IsRequired();

        builder.HasOne(answer => answer.Author)
            .WithMany()
            .HasForeignKey(answer => answer.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(answer => new { answer.QuestionId, answer.CreatedAt });

        builder.HasIndex(answer => answer.QuestionId)
            .IsUnique()
            .HasFilter("is_accepted = true")
            .HasDatabaseName("answers_one_accepted");
    }
}
