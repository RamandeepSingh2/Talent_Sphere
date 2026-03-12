using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;
using TalentSphere.Enums;

namespace TalentSphere.Config.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.ToTable("Enrollments");
            builder.HasKey(e => e.EnrollmentID);
            builder.Property(e => e.EnrollmentID).ValueGeneratedOnAdd();

            builder.Property(e => e.Date).IsRequired();

            builder.Property(e => e.status).HasConversion<string>().HasDefaultValue(EnrollmentStatus.Enrolled).IsRequired();

            builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property<bool>("IsDeleted").HasDefaultValue(false);

            builder.HasOne<Training>().WithMany().HasForeignKey(e => e.TrainingID).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Employee>().WithMany().HasForeignKey(e => e.EmployeeID).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
