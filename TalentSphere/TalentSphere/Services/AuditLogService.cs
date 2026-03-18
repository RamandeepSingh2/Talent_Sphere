using System;
using System.Threading.Tasks;
using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repository;
        private readonly IMapper _mapper;

        public AuditLogService(IAuditLogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AuditLogResponseDto> CreateAuditLogAsync(CreateAuditLogDTO dto)
        {
            var audit = _mapper.Map<AuditLog>(dto);
            audit.CreatedAt = DateTime.UtcNow;
            if (audit.Timestamp == default)
            {
                audit.Timestamp = DateTime.UtcNow;
            }

            var added = await _repository.AddAsync(audit);
            await _repository.SaveChangesAsync();
            return _mapper.Map<AuditLogResponseDto>(added);
        }

        public async Task<AuditLogResponseDto> GetByIdAsync(int id)
        {
            var audit = await _repository.GetByIdAsync(id);
            if (audit == null) return null;
            return _mapper.Map<AuditLogResponseDto>(audit);
        }

        public async Task<IEnumerable<AuditLogResponseDto>> GetAllAsync()
        {
            var audits = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<AuditLogResponseDto>>(audits);
        }

        public async Task<AuditLogResponseDto> UpdateAuditLogAsync(int id, UpdateAuditLogDTO dto)
        {
            var audit = await _repository.GetByIdAsync(id);
            if (audit == null) return null;

            // Apply partial updates
            if (dto.UserID.HasValue) audit.UserID = dto.UserID.Value;
            if (!string.IsNullOrWhiteSpace(dto.Action)) audit.Action = dto.Action;
            if (!string.IsNullOrWhiteSpace(dto.Resource)) audit.Resource = dto.Resource;
            if (dto.Timestamp.HasValue) audit.Timestamp = dto.Timestamp.Value;

            audit.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();

            return _mapper.Map<AuditLogResponseDto>(audit);
        }

        public async Task<bool> DeleteAuditLogAsync(int id)
        {
            var audit = await _repository.GetByIdAsync(id);
            if (audit == null) return false;

            audit.IsDeleted = true;
            audit.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
