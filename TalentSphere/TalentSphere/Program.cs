using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TalentSphere.Config;
using TalentSphere.Repositories;
using TalentSphere.Services.Interfaces;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services;
using TalentSphere.Enums;
using TalentSphere.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDb"))
           .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));


builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register Job repository and service
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IJobService, JobService>();

builder.Services.AddScoped<IInterviewRepository, InterviewRepository>();
builder.Services.AddScoped<IInterviewService, InterviewService>();

builder.Services.AddScoped<ISelectionRepository, SelectionRepository>();
// Register User repository and service
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Register Employee repository and service
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Register EmployeeDocument repository and service
builder.Services.AddScoped<IEmployeeDocumentRepository, EmployeeDocumentRepository>();
builder.Services.AddScoped<IEmployeeDocumentService, EmployeeDocumentService>();

// Register AuditLog repository and service
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// Register AuditLogHelper for simplified audit logging
builder.Services.AddScoped<AuditLogHelper>();

// Register UserRole repository and service
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();

builder.Services.AddScoped<IComplianceRecordRepository, ComplianceRecordRepository>();
builder.Services.AddScoped<IComplianceRecordService, ComplianceRecordService>();

builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IAuditService, AuditService>();

// Performance Review
builder.Services.AddScoped<IPerformanceReviewRepository, PerformanceReviewRepository>();
builder.Services.AddScoped<IPerformanceReviewService, PerformanceReviewService>();
// Career Plan
builder.Services.AddScoped<ICareerPlanRepository, CareerPlanRepository>();
builder.Services.AddScoped<ICareerPlanService, CareerPlanService>();
// Notification
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<IAuditService, AuditService>();

//Register report repository
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddScoped<ITrainingRepository, TrainingRepository>();
builder.Services.AddScoped<ITrainingService, TrainingService>();

builder.Services.AddScoped<ISuccessionPlanRepository, SuccessionPlanRepository>();
builder.Services.AddScoped<ISuccessionPlanService, SuccessionPlanService>();

builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();


builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();

builder.Services.AddScoped<IResumeRepository, ResumeRepository>();
builder.Services.AddScoped<IResumeService, ResumeService>();

builder.Services.AddScoped<IScreeningRepository, ScreeningRepository>();
builder.Services.AddScoped<IScreeningService, ScreeningService>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();

// Register TokenService for JWT token generation
builder.Services.AddScoped<TokenService>();

// Configure JWT Authentication
var key = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Handle unauthorized requests with custom JSON response
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsJsonAsync(new 
            { 
                message = "Unauthorized: Token is missing or invalid.",
                statusCode = 401
            });
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsJsonAsync(new 
            { 
                message = "Forbidden: You do not have permission to access this resource.",
                statusCode = 403
            });
        }
    };
});

// AutoMapper registration - scan assembly for profiles in the Mappers folder
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Seed default data (roles + admin) if configured
using (var scope = app.Services.CreateScope())
{
    try
    {
        var services = scope.ServiceProvider;
        var seederType = typeof(TalentSphere.Utilities.DataSeeder);
        var seedMethod = seederType.GetMethod("SeedAsync");
        if (seedMethod != null)
        {
            var task = (Task)seedMethod.Invoke(null, new object[] { services });
            task.GetAwaiter().GetResult();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Seed initial roles if they don't exist
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Ensure database is created
    await context.Database.MigrateAsync();

    // Check if roles already exist
    var existingRoles = await context.Roles.ToListAsync();

    if (!existingRoles.Any())
    {
        var roles = new List<Role>
        {
            new Role { Name = RoleName.Candidate, CreatedAt = DateTime.UtcNow },
            new Role { Name = RoleName.Recruiter, CreatedAt = DateTime.UtcNow },
            new Role { Name = RoleName.HR, CreatedAt = DateTime.UtcNow },
            new Role { Name = RoleName.Employee, CreatedAt = DateTime.UtcNow },
            new Role { Name = RoleName.Manager, CreatedAt = DateTime.UtcNow },
            new Role { Name = RoleName.Admin, CreatedAt = DateTime.UtcNow }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }
}

app.Run();
