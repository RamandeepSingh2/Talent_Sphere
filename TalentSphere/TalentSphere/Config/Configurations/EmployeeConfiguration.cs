using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;
using TalentSphere.Enums;

namespace TalentSphere.Config.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");
            builder.HasKey(e => e.EmployeeID);

            builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
            builder.Property(e => e.Department).HasMaxLength(100);
            builder.Property(e => e.Position).HasMaxLength(100);

            builder.Property(e => e.Status).HasDefaultValue(EmployeeStatus.Active).IsRequired();

            builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property<bool>("IsDeleted").HasDefaultValue(false);
        }
    }
}
