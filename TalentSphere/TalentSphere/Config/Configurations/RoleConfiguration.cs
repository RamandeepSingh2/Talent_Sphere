using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;

namespace TalentSphere.Config.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(r => r.RoleID);

            builder.Property(r => r.Name).IsRequired().HasMaxLength(50);

            builder.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(r => r.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property<bool>("IsDeleted").HasDefaultValue(false);
        }
    }
}
