using Microsoft.EntityFrameworkCore;
using TalentSphere.Models;
using TalentSphere.Config.Configurations;

namespace TalentSphere.Config
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new JobConfiguration());
            modelBuilder.ApplyConfiguration(new ResumeConfiguration());
            modelBuilder.ApplyConfiguration(new InterviewConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
            modelBuilder.ApplyConfiguration(new TrainingConfiguration());
            modelBuilder.ApplyConfiguration(new SuccessionPlanConfiguration());
            modelBuilder.ApplyConfiguration(new SelectionConfiguration());
            modelBuilder.ApplyConfiguration(new ScreeningConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new ComplianceRecordConfiguration());
            modelBuilder.ApplyConfiguration(new CareerPlanConfiguration());
            modelBuilder.ApplyConfiguration(new PerformanceReviewConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new ReportConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeDocConfiguration());
            modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
            modelBuilder.ApplyConfiguration(new AuditConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        }

        public DbSet<SuccessionPlan> SuccessionPlans { get; set; }
        public DbSet<Screening> Screenings { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<Selection> Selections { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ComplianceRecord> ComplianceRecords { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<CareerPlan> CareerPlans { get; set; }
        public DbSet<PerformanceReview> PerformanceReviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<EmployeeDocument> EmployeeDocs { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}
