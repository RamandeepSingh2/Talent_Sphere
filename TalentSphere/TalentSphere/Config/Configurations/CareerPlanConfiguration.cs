using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;
using TalentSphere.Enums;

namespace TalentSphere.Config.Configurations
{
    public class CareerPlanConfiguration : IEntityTypeConfiguration<CareerPlan>
    {
        public void Configure(EntityTypeBuilder<CareerPlan> builder)
        {
            builder.ToTable("CareerPlans");
            builder.HasKey(c => c.PlanID);
            builder.Property(c => c.PlanID).ValueGeneratedOnAdd();

            builder.Property(c => c.Goals)
                   .IsRequired()
                   .HasMaxLength(2000);

            // NEW: target role
            builder.Property(c => c.TargetRole)
                   .HasMaxLength(200)
                   .IsRequired(false);

            // NEW: target date
            builder.Property(c => c.TargetDate)
                   .IsRequired(false);

            // NEW: FK to PerformanceReview (nullable)
            builder.Property(c => c.ReviewID)
                   .IsRequired(false);

            builder.Property(c => c.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired()
                   .HasDefaultValue(CareerPlanStatus.Planned);

            builder.HasQueryFilter(c => !c.IsDeleted);

            builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(c => c.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(c => c.IsDeleted).HasDefaultValue(false);

            builder.HasOne(c => c.Employee)
                   .WithMany()
                   .HasForeignKey(c => c.EmployeeID)
                   .OnDelete(DeleteBehavior.Restrict);

            // NEW: optional FK to PerformanceReview
            builder.HasOne(c => c.Review)
                   .WithMany()
                   .HasForeignKey(c => c.ReviewID)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
