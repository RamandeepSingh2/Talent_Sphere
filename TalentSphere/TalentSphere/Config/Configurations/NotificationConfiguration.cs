using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentSphere.Models;
using TalentSphere.Enums;

namespace TalentSphere.Config.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");
            builder.HasKey(n => n.NotificationID);
            builder.Property(n => n.NotificationID).ValueGeneratedOnAdd();

            builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);
            builder.Property(n => n.Category)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(n => n.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .HasDefaultValue(NotificationStatus.Unread)
                   .IsRequired();

            builder.Property(n => n.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(n => n.UpdatedAt)
                     .HasDefaultValueSql("GETUTCDATE()")
                     .ValueGeneratedOnAddOrUpdate(); // Tells EF this value changes on every update

            builder.Property(n => n.IsDeleted)
                   .HasDefaultValue(false);

            // Global Filter: Deleted records automatically hides
            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserID).OnDelete(DeleteBehavior.Restrict);

            //  Performance Indexing
            // Index UserID to speed up fetching notifications for a specific user.
            builder.HasIndex(n => n.UserID);

           
        }
    }
}
