using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;
using TalentSphere.Enums;

namespace TalentSphere.Config.Configurations
{
    public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
    {
        public void Configure(EntityTypeBuilder<Resume> builder)
        {
            builder.ToTable("Resume");
            builder.HasKey(r => r.ResumeID);

            builder.Property(r => r.FileURI).IsRequired().HasMaxLength(500);

            builder.Property(r => r.Status)
                   .HasDefaultValue(ResumeStatus.Active)
                   .IsRequired();

            builder.Property(r => r.UploadedDate).HasDefaultValueSql("GETUTCDATE()");

            builder.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(r => r.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.Property<bool>("IsDeleted").HasDefaultValue(false);

            builder.HasOne(r => r.Candidate).WithMany().HasForeignKey(r => r.CandidateID).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
