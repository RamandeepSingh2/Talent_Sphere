using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;

namespace TalentSphere.Config.Configurations
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Reports");
            builder.HasKey(r => r.ReportID);
            builder.Property(r => r.ReportID).ValueGeneratedOnAdd();

            builder.Property(r => r.Scope).IsRequired().HasMaxLength(100);
            builder.Property(r => r.GenerateDate).IsRequired();

            builder.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(r => r.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property<bool>("IsDeleted").HasDefaultValue(false);
        }
    }
}
