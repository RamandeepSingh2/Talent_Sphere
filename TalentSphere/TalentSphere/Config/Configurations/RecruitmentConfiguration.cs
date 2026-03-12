using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;
using TalentSphere.Enums;

namespace TalentSphere.Config.Configurations
{
    public class RecruitmentConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("Recruitments");
            builder.HasKey(j => j.JobID);
            builder.Property(j => j.JobID).ValueGeneratedOnAdd();

            builder.Property(j => j.Title).IsRequired().HasMaxLength(255);
            builder.Property(j => j.Department).IsRequired().HasMaxLength(100);
            builder.Property(j => j.Description).HasMaxLength(2000);
            builder.Property(j => j.Requirements).HasMaxLength(2000);

            builder.Property(j => j.PostedDate)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(j => j.Status)
                   .HasDefaultValue(JobStatus.Open)
                   .IsRequired();

            builder.Property(j => j.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()")
                   .IsRequired();

            builder.Property(j => j.UpdatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(j => j.IsDeleted)
                   .HasDefaultValue(false)
                   .IsRequired();

            // No foreign keys defined for Job/Recruitment by default. Add HasOne(...) if needed.
        }
    }
}
