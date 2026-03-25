using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;

namespace TalentSphere.Config.Configurations
{
    public class PerformanceReviewConfiguration : IEntityTypeConfiguration<PerformanceReview>
    {
        public void Configure(EntityTypeBuilder<PerformanceReview> builder)
        {
            //Table
            builder.ToTable("PerformanceReviews");
            //key Mapping
            builder.HasKey(p => p.ReviewID);
            builder.Property(p => p.ReviewID).ValueGeneratedOnAdd();

            builder.Property(p => p.Score)
                   .HasColumnType("decimal(5,2)")
                   .IsRequired();

            builder.Property(p => p.Comments)
                   .HasMaxLength(2000);

            builder.Property(p => p.Date)
                               .HasColumnType("datetime")
                               .IsRequired();
            // Timestamps
            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();

            builder.Property(p => p.UpdatedAt)
                   .HasDefaultValueSql("GETUTCDATE()")
                   .ValueGeneratedOnAddOrUpdate();

            //  Soft Delete Logic
            builder.Property(p => p.IsDeleted)
                   .HasDefaultValue(false);

            // This line automatically hides deleted records from all queries
            builder.HasQueryFilter(p => !p.IsDeleted);


            builder.HasOne(p => p.Employee)
                   .WithMany()
                   .HasForeignKey(p => p.EmployeeID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Manager)
                   .WithMany()
                   .HasForeignKey(p => p.ManagerID)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
