using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;
using TalentSphere.Enums;

namespace TalentSphere.Config.Configurations
{
    public class TrainingConfiguration : IEntityTypeConfiguration<Training>
    {
        public void Configure(EntityTypeBuilder<Training> builder)
        {
            builder.ToTable("Trainings");
            builder.HasKey(t => t.TrainingID);
            builder.Property(t => t.TrainingID).ValueGeneratedOnAdd();

            builder.Property(t => t.Title).IsRequired().HasMaxLength(255);
            builder.Property(t => t.Duration).IsRequired().HasMaxLength(100);

            builder.Property(t => t.status).HasConversion<string>().HasDefaultValue(TrainingStatus.Planned).IsRequired();

            builder.Property(t => t.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(t => t.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property<bool>("IsDeleted").HasDefaultValue(false);
        }
    }
}
