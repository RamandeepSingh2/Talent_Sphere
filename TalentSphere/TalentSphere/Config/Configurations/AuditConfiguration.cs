using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;
using TalentSphere.Enums;

namespace TalentSphere.Config.Configurations
{
    public class AuditConfiguration : IEntityTypeConfiguration<Audit>
    {
        public void Configure(EntityTypeBuilder<Audit> builder)
        {
            builder.ToTable("Audits");
            builder.HasKey(a => a.AuditID);
            builder.Property(a => a.AuditID).ValueGeneratedOnAdd();

            builder.Property(a => a.Scope).IsRequired().HasMaxLength(255);
            builder.Property(a => a.Findings).HasMaxLength(1000);
            builder.Property(a => a.Date).IsRequired();
            builder.Property(a => a.Status)
                   .HasConversion<string>()
                   .HasDefaultValue(AuditStatus.Pending)
                   .IsRequired();
            builder.Property(a => a.IsDeleted)
                   .HasDefaultValue(false)
                   .IsRequired();
            builder.Property(a => a.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(a => a.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(a => a.HR)
                   .WithMany()
                   .HasForeignKey(a => a.HRID)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
