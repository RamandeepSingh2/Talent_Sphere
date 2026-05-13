using AutoMapper;

using TalentSphere.DTOs;
using TalentSphere.DTOs.PerformanceReview;
using TalentSphere.DTOs.CareerPlan;
using TalentSphere.DTOs.Notification;
using TalentSphere.Enums;
using TalentSphere.Models;
using BC = BCrypt.Net.BCrypt;

namespace TalentSphere.Mappers
{
    public class ConfigurationMapperProfile : Profile
    {
        public ConfigurationMapperProfile()
        {

            // ComplianceRecord mappings
            CreateMap<CreateComplianceRecordDTO, ComplianceRecord>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => ParseComplianceType(src.RecordType)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Result, opt => opt.MapFrom(src => src.Result ?? "Pending"))  // use what user sends
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date ?? DateTime.UtcNow))  // use what user sends
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateComplianceRecordDTO, ComplianceRecord>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => ParseComplianceType(src.RecordType)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Result, opt => opt.MapFrom(src => src.Result))  // map result from DTO
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))    // map date from DTO
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ComplianceRecord, ComplianceRecordResponseDTO>()
                .ForMember(dest => dest.RecordType, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.Result, opt => opt.MapFrom(src => src.Result))   // ADD
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))      // ADD
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Name : null));

            // Audit mappings
            CreateMap<CreateAuditDTO, Audit>()
                .ForMember(dest => dest.Scope, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.AuditDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseAuditStatus(src.Status)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateAuditDTO, Audit>()
                .ForMember(dest => dest.Scope, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.AuditDate ?? DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseAuditStatus(src.Status)))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Audit, AuditResponseDTO>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Scope))
                .ForMember(dest => dest.AuditDate, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            // Job mappings
            CreateMap<CreateJobDTO, Job>().ReverseMap();
            CreateMap<UpdateJobDTO, Job>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Interview mappings
            CreateMap<CreateInterviewDTO, Interview>().ReverseMap();
            CreateMap<UpdateInterviewDTO, Interview>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Selection mappings
            CreateMap<CreateSelectionDTO, Selection>()
                .ForMember(dest => dest.Decision, opt => opt.MapFrom(src => ParseSelectionDecision(src.Decision)))
                .ReverseMap();
            CreateMap<UpdateSelectionDTO, Selection>()
                .ForMember(dest => dest.Decision, opt => opt.MapFrom(src => ParseSelectionDecision(src.Decision)))
                .ReverseMap();


            //PerformanceReview Mapping
            CreateMap<CreatePerformanceReviewDTO, PerformanceReview>()
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => (decimal)src.Rating))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.ReviewDate))
                .ForMember(dest => dest.ManagerID, opt => opt.Ignore());

            CreateMap<UpdatePerformanceReviewDTO, PerformanceReview>()
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Rating != null ? (decimal)src.Rating.Value : 0))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.ReviewDate ?? DateTime.UtcNow))
                .ForMember(dest => dest.ReviewID, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeID, opt => opt.Ignore())
                .ForMember(dest => dest.ManagerID, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<PerformanceReview, PerformanceReviewDTO>()
                    .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => (int)src.Score))
                    .ForMember(dest => dest.ReviewDate, opt => opt.MapFrom(src => src.Date))
                    .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Name : null))
                    .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.Name : null));

            CreateMap<PerformanceReview, PerformanceReviewListDTO>()
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => (int)src.Score))
                .ForMember(dest => dest.ReviewDate, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Name : null))
                .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.Name : null));

            //Notification Mapping
            CreateMap<CreateNotificationDTO, Notification>()
                .ForMember(d => d.IsDeleted, opt => opt.MapFrom(s => false));

            CreateMap<Notification, NotificationResponseDTO>()
                .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category.ToString()))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
            // CareerPlan Mapping Configuration
            CreateMap<CreateCareerPlanDTO, CareerPlan>()
     .ForMember(dest => dest.Goals, opt => opt.MapFrom(src => src.Goals))
     .ForMember(dest => dest.TargetRole, opt => opt.MapFrom(src => src.TargetRole))
     .ForMember(dest => dest.TargetDate, opt => opt.MapFrom(src => src.TargetDate))
     .ForMember(dest => dest.ReviewID, opt => opt.MapFrom(src => src.ReviewID));

            CreateMap<UpdateCareerPlanDTO, CareerPlan>()
                .ForMember(dest => dest.Goals, opt => opt.MapFrom(src => src.Goals))
                .ForMember(dest => dest.TargetRole, opt => opt.MapFrom(src => src.TargetRole))
                .ForMember(dest => dest.TargetDate, opt => opt.MapFrom(src => src.TargetDate));

            CreateMap<CareerPlan, CareerPlanResponseDTO>()
                .ForMember(dest => dest.Goals, opt => opt.MapFrom(src => src.Goals))
                .ForMember(dest => dest.TargetRole, opt => opt.MapFrom(src => src.TargetRole))
                .ForMember(dest => dest.TargetDate, opt => opt.MapFrom(src => src.TargetDate))
                .ForMember(dest => dest.ReviewID, opt => opt.MapFrom(src => src.ReviewID))
                .ForMember(dest => dest.ReviewPeriod, opt => opt.MapFrom(src => src.Review != null ? src.Review.ReviewPeriod : null))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Name : null));

            // Notification Mapping Configuration
            //CreateMap<CreateNotificationDTO, Notification>()
            //    .ForMember(dest => dest.NotificationID, opt => opt.Ignore())
            //    .ForMember(dest => dest.Status, opt => opt.Ignore())      
            //    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())   
            //    .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())   
            //    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())   
            //    .ForMember(dest => dest.User, opt => opt.Ignore())
            //    .ReverseMap();
            // User mappings

            // RegisterDTO mapping with password hashing
            CreateMap<RegisterDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => BC.HashPassword(src.Password)))
                .ForMember(dest => dest.UserID, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            // Update mapping: map only provided (non-null and non-whitespace for strings) fields
            // NOTE: Avoid chaining across calls that may return void; assign to local variable first to prevent CS0023.
            var updateUserMap = CreateMap<UpdateUserDTO, User>();
            // Use the 4-argument Condition overload to be compatible with AutoMapper signatures
            updateUserMap.ForAllMembers(opt => opt.Condition((src, dest, srcMember, context) =>
                srcMember != null && (!(srcMember is string) || !string.IsNullOrWhiteSpace((string)srcMember))
            ));
            
            // Employee mappings
            CreateMap<CreateEmployeeDTO, Employee>()
                .ReverseMap();

            // Update mapping for Employee: map only provided fields (non-null and non-whitespace for strings)
            var updateEmployeeMap = CreateMap<UpdateEmployeeDTO, Employee>();
            updateEmployeeMap.ForAllMembers(opt => opt.Condition((src, dest, srcMember, context) =>
                srcMember != null && (!(srcMember is string) || !string.IsNullOrWhiteSpace((string)srcMember))
            ));

            // Employee response mapping
            CreateMap<Employee, EmployeeResponseDto>()
                .ForMember(dest => dest.Email,       opt => opt.MapFrom((src, _) => src.User    != null ? src.User.Email    : string.Empty))
                .ForMember(dest => dest.Phone,       opt => opt.MapFrom((src, _) => src.User    != null ? src.User.Phone    : string.Empty))
                .ForMember(dest => dest.ManagerName, opt => opt.MapFrom((src, _) => src.Manager != null ? src.Manager.Name  : string.Empty));

            // User mappings
            CreateMap<User, UserResponseDto>()
                .ReverseMap();
            CreateMap<CreateUserDTO, User>().ReverseMap();
            CreateMap<UpdateUserDTO, User>().ReverseMap();

            // EmployeeDocument mappings
            CreateMap<CreateEmployeeDocumentDTO, EmployeeDocument>()
                .ReverseMap();

            // EmployeeDocument -> Response DTO
            CreateMap<EmployeeDocument, EmployeeDocumentResponseDto>()
                .ReverseMap();

            // AuditLog -> Response DTO
            CreateMap<AuditLog, AuditLogResponseDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.Name : null));

            // UserRole mappings
            CreateMap<CreateUserRoleDTO, UserRole>()
                .ReverseMap();

            // UserRole -> Response DTO
            CreateMap<UserRole, UserRoleResponseDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role == null ? string.Empty : src.Role.Name.ToString()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User == null ? string.Empty : src.User.Name));
            //Application mappings
            CreateMap<CreateApplicationDTO, Application>()
                .ReverseMap();

            // Application -> Response DTO
            CreateMap<Application, ApplicationResponseDTO>()
                .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => src.Candidate != null ? src.Candidate.Name : null))
                .ForMember(dest => dest.JobTitle,      opt => opt.MapFrom(src => src.Job      != null ? src.Job.Title      : null))
                .ReverseMap();
            // UpdateApplicationDTO -> Application (skip nulls)
            var updateApplicationMap = CreateMap<UpdateApplicationDTO, Application>();
            updateApplicationMap.ForAllMembers(opt => opt.Condition((src, dest, srcMember, context) =>
                srcMember != null && (!(srcMember is string) || !string.IsNullOrWhiteSpace((string)srcMember))
            ));

            //Resume mappings
            CreateMap<CreateResumeDTO, Resume>()
                .ReverseMap();
            // Resume -> Response DTO
            CreateMap<Resume, ResumeResponseDTO>()
                .ReverseMap();
            // UpdateResumeDTO -> Resume (skip nulls)
            var updateResumeMap = CreateMap<UpdateResumeDTO, Resume>();
            updateResumeMap.ForAllMembers(opt => opt.Condition((src, dest, srcMember, context) =>
                srcMember != null && (!(srcMember is string) || !string.IsNullOrWhiteSpace((string)srcMember))
            ));

            //Screening mappings
            CreateMap<CreateScreeningDTO, Screening>()
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Feedback))
                .ForMember(dest => dest.Result, opt => opt.MapFrom(src => src.Result))
                .ForMember(dest => dest.Date, opt => opt.Ignore());

            // Screening -> Response DTO
            CreateMap<Screening, ScreeningResponseDTO>()
                .ForMember(dest => dest.Feedback, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.Result, opt => opt.MapFrom(src => src.Result.ToString()))
                .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src =>
                    src.Application != null && src.Application.Candidate != null ? src.Application.Candidate.Name : null))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src =>
                    src.Application != null && src.Application.Job != null ? src.Application.Job.Title : null));

            // UpdateScreeningDTO -> Screening (skip nulls)
            var updateScreeningMap = CreateMap<UpdateScreeningDTO, Screening>();
            updateScreeningMap
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Feedback))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember, context) =>
                    srcMember != null && (!(srcMember is string) || !string.IsNullOrWhiteSpace((string)srcMember))
                ));
            //Report mappings
            CreateMap<CreateReportDTO, Report>()
                .ReverseMap();

            // Training and Enrollment mappings are handled manually in their services

            // SuccessionPlan mappings are handled manually in SuccessionPlanService

            //Role mappings
            CreateMap<CreateRoleDTO, Role>().ReverseMap();
            CreateMap<UpdateRoleDTO, Role>().ReverseMap();
            CreateMap<Role, RoleResponseDTO>().ReverseMap();
        }

        private static TalentSphere.Enums.SelectionDecision ParseSelectionDecision(string value)
        {
            return Enum.TryParse<TalentSphere.Enums.SelectionDecision>(value, true, out var result)
                ? result
                : TalentSphere.Enums.SelectionDecision.Rejected;
        }

        private static TalentSphere.Enums.CompilanceRecordType ParseComplianceType(string value)
        {
            return Enum.TryParse<TalentSphere.Enums.CompilanceRecordType>(value, true, out var result)
                ? result
                : TalentSphere.Enums.CompilanceRecordType.Certificate;
        }

        private static TalentSphere.Enums.AuditStatus ParseAuditStatus(string value)
        {
            return Enum.TryParse<TalentSphere.Enums.AuditStatus>(value, true, out var result)
                ? result
                : TalentSphere.Enums.AuditStatus.Active;
        }
    }
}
