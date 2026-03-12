using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;
using TalentSphere.Enums;

namespace TalentSphere.Config.Configurations
{
    public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
    {
        public void Configure(EntityTypeBuilder<Interview> builder)
        {
            builder.ToTable("Interviews");
            builder.HasKey(i => i.InterviewID);
            builder.Property(i => i.InterviewID).ValueGeneratedOnAdd();

            builder.Property(i => i.Date).IsRequired();
            builder.Property(i => i.Time).IsRequired();

            builder.Property(i => i.Status).HasConversion<string>().HasDefaultValue(InterviewStatus.Scheduled).IsRequired();

            builder.Property(i => i.IsDeleted).HasDefaultValue(false).IsRequired();

            builder.Property(i => i.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(i => i.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(i => i.Application).WithMany().HasForeignKey(i => i.ApplicationID).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(i => i.Interviewer).WithMany().HasForeignKey(i => i.InterviewerID).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
