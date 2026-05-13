using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;

namespace TalentSphere.Config.Configurations
{
    public class PerformanceReviewConfiguration : IEntityTypeConfiguration<PerformanceReview>
    {
        public void Configure(EntityTypeBuilder<PerformanceReview> builder)
        {
            builder.ToTable("PerformanceReviews");
            builder.HasKey(p => p.ReviewID);
            builder.Property(p => p.ReviewID).ValueGeneratedOnAdd();

            builder.Property(p => p.Score)
                   .HasColumnType("decimal(5,2)")
                   .IsRequired();

            builder.Property(p => p.Comments).HasMaxLength(2000);

            // NEW
            builder.Property(p => p.ReviewPeriod).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.AreasToImprove).HasMaxLength(2000).IsRequired(false);

            builder.Property(p => p.Date).HasColumnType("datetime").IsRequired();

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()")
                   .ValueGeneratedOnAdd();

            builder.Property(p => p.UpdatedAt)
                   .HasDefaultValueSql("GETUTCDATE()")
                   .ValueGeneratedOnAddOrUpdate();

            builder.Property(p => p.IsDeleted).HasDefaultValue(false);

            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.HasOne(p => p.Employee)
                   .WithMany()
                   .HasForeignKey(p => p.EmployeeID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Manager)
                   .WithMany()
                   .HasForeignKey(p => p.ManagerID)
                   .OnDelete(DeleteBehavior.Restrict);

            // CareerPlanID FK removed — link goes one way only:
            // CareerPlan.ReviewID → PerformanceReview
            // No reverse FK needed here
        }
    }
}