using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Enums;
using TalentSphere.Models;

namespace TalentSphere.Config.Configurations
{
    public class EmployeeDocConfiguration : IEntityTypeConfiguration<EmployeeDocument>
    {
        public void Configure(EntityTypeBuilder<EmployeeDocument> builder)
        {
            builder.ToTable("EmployeeDocs");
            builder.HasKey(d => d.DocumentID);
            builder.Property(d => d.DocumentID).ValueGeneratedOnAdd();

            // Store DocType enum as string in DB
            builder.Property(d => d.DocType)
                .HasConversion<string>()
                .HasMaxLength(100);
            builder.Property(d => d.FileURI).HasMaxLength(500);
            // Map VerifyStatus enum to string in the DB, keep max length and default
            builder.Property(e => e.VerifyStatus)
                .HasConversion<string>()
                .HasDefaultValue(EmployeeDocVerifyStatus.Pending);

            builder.Property(d => d.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(d => d.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property<bool>("IsDeleted").HasDefaultValue(false);

            builder.HasOne(d => d.Employee).WithMany().HasForeignKey(d => d.EmployeeID).OnDelete(DeleteBehavior.Restrict).IsRequired();
        }
    }
}
