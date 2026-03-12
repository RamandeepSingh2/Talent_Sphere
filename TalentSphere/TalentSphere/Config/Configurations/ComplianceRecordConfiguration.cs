using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;

namespace TalentSphere.Config.Configurations
{
    public class ComplianceRecordConfiguration : IEntityTypeConfiguration<ComplianceRecord>
    {
        public void Configure(EntityTypeBuilder<ComplianceRecord> builder)
        {
            builder.ToTable("ComplianceRecords");
            builder.HasKey(c => c.ComplianceID);
            builder.Property(c => c.ComplianceID).ValueGeneratedOnAdd();

            builder.Property(c => c.Type).IsRequired();
            builder.Property(c => c.Date).IsRequired();

            builder.Property(c => c.IsDeleted)
                   .HasDefaultValue(false)
                   .IsRequired();

            builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(c => c.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(c => c.Employee).WithMany().HasForeignKey(c => c.EmployeeID).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
