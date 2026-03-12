using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;

namespace TalentSphere.Config.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");
            builder.HasKey(a => a.AuditID);
            builder.Property(a => a.AuditID).ValueGeneratedOnAdd();

            builder.Property(a => a.Action).IsRequired().HasMaxLength(255);
            builder.Property(a => a.Resource).IsRequired().HasMaxLength(255);

            builder.Property(a => a.Timestamp).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(a => a.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(a => a.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property<bool>("IsDeleted").HasDefaultValue(false);
            builder.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserID).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
