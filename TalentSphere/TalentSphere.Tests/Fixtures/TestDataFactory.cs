using TalentSphere.DTOs;
using TalentSphere.DTOs.Notification;
using TalentSphere.Enums;
using TalentSphere.Models;

namespace TalentSphere.Tests.Fixtures
{
    /// <summary>
    /// Static factory class for creating consistent test data objects.
    /// </summary>
    public static class TestDataFactory
    {
        // ──────────────────────────────────────────────────────────────
        // Models
        // ──────────────────────────────────────────────────────────────

        public static User CreateUser(
            int id = 1,
            string name = "John Doe",
            string email = "john.doe@example.com",
            string? phone = "1234567890",
            UserStatus status = UserStatus.Active,
            bool isDeleted = false)
        {
            return new User
            {
                UserID = id,
                Name = name,
                Email = email,
                // Pre-hashed "Test@123" using BCrypt with a fixed work factor for reproducibility
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                Phone = phone,
                Status = status,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = null,
                IsDeleted = isDeleted
            };
        }

        public static Employee CreateEmployee(
            int id = 1,
            int userId = 1,
            string name = "Jane Smith",
            string department = "Engineering",
            string position = "Software Engineer",
            EmployeeStatus status = EmployeeStatus.Active,
            int? managerId = null,
            bool isDeleted = false)
        {
            return new Employee
            {
                EmployeeID = id,
                UserId = userId,
                Name = name,
                Department = department,
                Position = position,
                JoinDate = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                Status = status,
                ManagerID = managerId,
                CreatedAt = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = null,
                IsDeleted = isDeleted
            };
        }

        public static Job CreateJob(
            int id = 1,
            string title = "Senior Developer",
            string department = "Engineering",
            string description = "Develop and maintain software applications.",
            string requirements = "5+ years C# experience.",
            JobStatus status = JobStatus.Open,
            bool isDeleted = false)
        {
            return new Job
            {
                JobID = id,
                Title = title,
                Department = department,
                Description = description,
                Requirements = requirements,
                PostedDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                Status = status,
                CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = null,
                IsDeleted = isDeleted
            };
        }

        public static Application CreateApplication(
            int id = 1,
            int jobId = 1,
            int candidateId = 1,
            ApplicationStatus status = ApplicationStatus.Submitted,
            bool isDeleted = false)
        {
            return new Application
            {
                ApplicationID = id,
                JobID = jobId,
                CandidateID = candidateId,
                SubmittedDate = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                Status = status,
                CreatedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = null,
                IsDeleted = isDeleted
            };
        }

        public static Role CreateRole(int id = 1, RoleName name = RoleName.Employee)
        {
            return new Role
            {
                RoleID = id,
                Name = name,
                IsDeleted = false,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
        }

        public static UserRole CreateUserRole(int id = 1, int userId = 1, int roleId = 1, RoleName roleName = RoleName.Candidate)
        {
            return new UserRole
            {
                UserRoleId = id,
                UserId = userId,
                RoleId = roleId,
                IsDeleted = false,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Role = new Role { RoleID = roleId, Name = roleName, IsDeleted = false, CreatedAt = DateTime.UtcNow }
            };
        }

        // ──────────────────────────────────────────────────────────────
        // DTOs
        // ──────────────────────────────────────────────────────────────

        public static RegisterDTO CreateRegisterDTO(
            string name = "John Doe",
            string email = "john.doe@example.com",
            string password = "Test@123",
            string? phone = "1234567890")
        {
            return new RegisterDTO
            {
                Name = name,
                Email = email,
                Password = password,
                Phone = phone
            };
        }

        public static LoginDTO CreateLoginDTO(
            string email = "john.doe@example.com",
            string password = "Test@123")
        {
            return new LoginDTO
            {
                Email = email,
                Password = password
            };
        }

        public static CreateEmployeeDTO CreateEmployeeDTO(
            int userId = 1,
            string name = "Jane Smith",
            string department = "Engineering",
            string position = "Software Engineer",
            EmployeeStatus status = EmployeeStatus.Active,
            int? managerId = null)
        {
            return new CreateEmployeeDTO
            {
                UserId = userId,
                Name = name,
                Department = department,
                Position = position,
                JoinDate = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                Status = status,
                ManagerID = managerId
            };
        }

        public static CreateJobDTO CreateJobDTO(
            string title = "Senior Developer",
            string department = "Engineering",
            string description = "Develop and maintain software applications.",
            string requirements = "5+ years C# experience.")
        {
            return new CreateJobDTO
            {
                Title = title,
                Department = department,
                Description = description,
                Requirements = requirements
            };
        }

        public static UserResponseDto CreateUserResponseDto(
            int id = 1,
            string name = "John Doe",
            string email = "john.doe@example.com",
            UserStatus status = UserStatus.Active)
        {
            return new UserResponseDto
            {
                UserID = id,
                Name = name,
                Email = email,
                Status = status,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
        }

        public static EmployeeResponseDto CreateEmployeeResponseDto(
            int id = 1,
            int userId = 1,
            string name = "Jane Smith",
            string department = "Engineering",
            string position = "Software Engineer",
            EmployeeStatus status = EmployeeStatus.Active)
        {
            return new EmployeeResponseDto
            {
                EmployeeID = id,
                UserId = userId,
                Name = name,
                Department = department,
                Position = position,
                JoinDate = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                Status = status,
                CreatedAt = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc)
            };
        }

        public static ApplicationResponseDTO CreateApplicationResponseDTO(
            int id = 1,
            int jobId = 1,
            int candidateId = 1,
            string jobTitle = "Senior Developer",
            string candidateName = "John Doe",
            ApplicationStatus status = ApplicationStatus.Submitted)
        {
            return new ApplicationResponseDTO
            {
                ApplicationID = id,
                JobID = jobId,
                CandidateID = candidateId,
                JobTitle = jobTitle,
                CandidateName = candidateName,
                SubmittedDate = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                Status = status,
                CreatedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            };
        }

        public static NotificationResponseDTO CreateNotificationResponseDTO(
            int id = 1,
            int entityId = 1,
            string message = "Welcome!",
            string category = "System",
            string status = "Unread")
        {
            return new NotificationResponseDTO
            {
                NotificationID = id,
                EntityID = entityId,
                Message = message,
                Category = category,
                Status = status,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
        }
    }
}
