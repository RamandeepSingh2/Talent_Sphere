using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;
using TalentSphere.Enums;

namespace TalentSphere.Config.Configurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.ToTable("Application");
            builder.HasKey(a => a.ApplicationID);
            builder.Property(a => a.ApplicationID).ValueGeneratedOnAdd();

            builder.Property(a => a.SubmittedDate).IsRequired();

            builder.Property(a => a.Status)
                   .HasConversion<string>()
                   .HasDefaultValue(ApplicationStatus.Pending)
                   .IsRequired();

            builder.Property(a => a.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(a => a.UpdatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property<bool>("IsDeleted").HasDefaultValue(false);

            builder.HasOne(a => a.Job).WithMany().HasForeignKey(a => a.JobID).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(a => a.Candidate).WithMany().HasForeignKey(a => a.CandidateID).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
