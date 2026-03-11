using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;

namespace TalentSphere.Config.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.UserID);

            builder.Property(u => u.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Phone)
                   .HasMaxLength(50);

            builder.HasIndex(u => u.Phone).IsUnique();

            builder.Property(u => u.Status)
                   .HasMaxLength(50)
                   .HasDefaultValue("Active");

            builder.Property(u => u.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.UpdatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            // Ensure IsDeleted has a default value of false (shadow property if not present on the CLR type)
            builder.Property<bool>("IsDeleted").HasDefaultValue(false);

            //builder.HasOne(u => u.Role)
            //       .WithMany(r => r.Users)
            //       .HasForeignKey(u => u.RoleID)
            //       .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
