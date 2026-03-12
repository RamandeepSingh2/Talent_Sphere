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

            builder.Property(c => c.Goals).IsRequired();
            builder.Property(c => c.Timeline).HasMaxLength(255);


            // --- Enum Configuration ---
            builder.Property(c => c.Status)
                   .HasConversion<string>() 
                   .HasMaxLength(50)
                   .IsRequired()
                   .HasDefaultValue(CareerPlanStatus.Draft);

            

            // Global Filter: Automatically hides IsDeleted = true from all queries
            builder.HasQueryFilter(c => !c.IsDeleted);

            builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(c => c.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            // --- Soft Delete Configuration ---
            builder.Property(c => c.IsDeleted)
                   .HasDefaultValue(false);

            //here we set the relationship between CareerPlan and Employee, with a foreign key of EmployeeID. We also specify that if an Employee is deleted, we want to restrict the deletion of related CareerPlans (i.e., prevent cascading deletes).
            builder.HasOne(c => c.Employee).WithMany().HasForeignKey(c => c.EmployeeID).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
