using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;
using TalentSphere.Enums;

namespace TalentSphere.Config.Configurations
{
    public class ScreeningConfiguration : IEntityTypeConfiguration<Screening>
    {
        public void Configure(EntityTypeBuilder<Screening> builder)
        {
            builder.ToTable("Screenings");
            builder.HasKey(s => s.ScreeningID);

            builder.Property(s => s.Result).IsRequired();

            builder.Property(s => s.Date).HasDefaultValueSql("GETUTCDATE()");

            builder.Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(s => s.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.Property<bool>("IsDeleted").HasDefaultValue(false);

            builder.HasOne(s => s.Application).WithMany().HasForeignKey(s => s.ApplicationID).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(s => s.Recruiter).WithMany().HasForeignKey(s => s.RecruiterID).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
