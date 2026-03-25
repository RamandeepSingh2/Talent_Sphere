using AutoMapper;

using TalentSphere.DTOs;
using TalentSphere.DTOs.PerformanceReview;
using TalentSphere.DTOs.CareerPlan;
using TalentSphere.DTOs.Notification;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Utils;

namespace TalentSphere.Mappers
{
    public class ConfigurationMapperProfile : Profile
    {
        public ConfigurationMapperProfile()
        {

            // ComplianceRecord mappings
            CreateMap<CreateComplianceRecordDTO, ComplianceRecord>()
                .ReverseMap();

            CreateMap<UpdateComplianceRecordDTO, ComplianceRecord>()
                .ReverseMap();

            CreateMap<ComplianceRecord, ComplianceRecordResponseDTO>()
                .ReverseMap();


            // Audit mappings
            CreateMap<CreateAuditDTO, Audit>()
                .ReverseMap();

            CreateMap<UpdateAuditDTO, Audit>()
                .ReverseMap();

            CreateMap<Audit, AuditResponseDTO>()

                .ReverseMap();
            // Job mappings
            CreateMap<CreateJobDTO, Job>()
                .ReverseMap();

            // Interview mappings
            CreateMap<CreateInterviewDTO, Interview>()
                .ReverseMap();

            // Selection mappings
            CreateMap<CreateSelectionDTO, Selection>()
                .ReverseMap();


            //PerformanceReview Mapping
            // PerformanceReview Mapping Cfor Create
            CreateMap<CreatePerformanceReviewDTO, PerformanceReview>();

            //Performancereview Mapping for Update 
            CreateMap<UpdatePerformanceReviewDTO, PerformanceReview>()
             .ForMember(dest => dest.ReviewID, opt => opt.Ignore())
             .ForMember(dest => dest.EmployeeID, opt => opt.Ignore())
             .ForMember(dest => dest.ManagerID, opt => opt.Ignore())
             .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
             .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            // PerformanceReview Mapping for Read
            CreateMap<PerformanceReview, PerformanceReviewDTO>();
            CreateMap<PerformanceReview, PerformanceReviewListDTO>();

            //Notification Mapping
            CreateMap<CreateNotificationDTO, Notification>()
                .ForMember(d => d.IsDeleted, opt => opt.MapFrom(s => false));

            CreateMap<Notification, NotificationResponseDTO>()
                .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category.ToString()))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
            // CareerPlan Mapping Configuration
            CreateMap<CreateCareerPlanDTO, CareerPlan>();
            CreateMap<UpdateCareerPlanDTO, CareerPlan>();

            CreateMap<CareerPlan, CareerPlanResponseDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.Name));

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
            CreateMap<CreateUserDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => PasswordHasher.Hash(src.PasswordHash)));

            // Update mapping: map only provided (non-null and non-whitespace for strings) fields
            // NOTE: Avoid chaining across calls that may return void; assign to local variable first to prevent CS0023.
            var updateUserMap = CreateMap<UpdateUserDTO, User>();
            // Use the 4-argument Condition overload to be compatible with AutoMapper signatures
            updateUserMap.ForAllMembers(opt => opt.Condition((src, dest, srcMember, context) =>
                srcMember != null && (!(srcMember is string) || !string.IsNullOrWhiteSpace((string)srcMember))
            ));
            // Rely on the ForAllMembers condition to skip null/whitespace strings; map/hash password when present
            updateUserMap.ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => PasswordHasher.Hash(src.PasswordHash)));

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
                .ReverseMap();

            // User mappings
            CreateMap<User, UserResponseDto>()
                .ReverseMap();

            // EmployeeDocument mappings
            CreateMap<CreateEmployeeDocumentDTO, EmployeeDocument>()
                .ReverseMap();

            // EmployeeDocument -> Response DTO
            CreateMap<EmployeeDocument, EmployeeDocumentResponseDto>()
                .ReverseMap();

            // AuditLog -> Response DTO
            CreateMap<AuditLog, AuditLogResponseDto>()
                .ReverseMap();

            // UserRole mappings
            CreateMap<CreateUserRoleDTO, UserRole>()
                .ReverseMap();

            // UserRole -> Response DTO
            CreateMap<UserRole, UserRoleResponseDto>()
                .ReverseMap();
            //Application mappings
            CreateMap<CreateApplicationDTO, Application>()
                .ReverseMap();

            // Application -> Response DTO
            CreateMap<Application, ApplicationResponseDTO>()
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
                .ReverseMap();
            // Screening -> Response DTO
            CreateMap<Screening, ScreeningResponseDTO>()
                .ReverseMap();
            // UpdateScreeningDTO -> Screening (skip nulls)
            var updateScreeningMap = CreateMap<UpdateScreeningDTO, Screening>();
            updateScreeningMap.ForAllMembers(opt => opt.Condition((src, dest, srcMember, context) =>
                srcMember != null && (!(srcMember is string) || !string.IsNullOrWhiteSpace((string)srcMember))
            ));
            //Report mappings
            CreateMap<CreateReportDTO, Report>()
                .ReverseMap();

            //Training mappings
            CreateMap<CreateTrainingDTO, Training>()
                     .ReverseMap();

            //Enrollment mappings
            CreateMap<CreateEnrollmentDTO, Enrollment>()
                .ReverseMap();

            //SuccessionPlan mappings
            CreateMap<CreateSuccessionPlanDTO, SuccessionPlan>()
                    .ReverseMap();

        }
    }
}
