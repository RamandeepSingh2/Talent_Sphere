using System;
using System.Threading.Tasks;
using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Services.Interfaces;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Services
{
    public class EmployeeDocumentService : IEmployeeDocumentService
    {
        private readonly IEmployeeDocumentRepository _repository;
        private readonly IMapper _mapper;

        public EmployeeDocumentService(IEmployeeDocumentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EmployeeDocumentResponseDto> CreateEmployeeDocumentAsync(CreateEmployeeDocumentDTO dto)
        {
            var doc = _mapper.Map<EmployeeDocument>(dto);
            doc.CreatedAt = DateTime.UtcNow;

            doc.FileURI = "...";

            var added = await _repository.AddAsync(doc);
            return _mapper.Map<EmployeeDocumentResponseDto>(added);
        }

        public async Task<EmployeeDocumentResponseDto> GetByIdAsync(int id)
        {
            var doc = await _repository.GetByIdAsync(id);
            if (doc == null) return null;
            return _mapper.Map<EmployeeDocumentResponseDto>(doc);
        }
        
        public async Task<IEnumerable<EmployeeDocumentResponseDto>> GetAllAsync()
        {
            var docs = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<EmployeeDocumentResponseDto>>(docs);
        }

        public async Task<EmployeeDocumentResponseDto> UpdateEmployeeDocumentAsync(int id, UpdateEmployeeDocumentDTO dto)
        {
            var doc = await _repository.GetByIdAsync(id);
            if (doc == null) return null;

            if (dto.EmployeeID.HasValue) doc.EmployeeID = dto.EmployeeID.Value;
            if (!string.IsNullOrWhiteSpace(dto.FileURI)) doc.FileURI = dto.FileURI;
            if (dto.UploadedDate.HasValue) doc.UploadedDate = dto.UploadedDate.Value;
            if (dto.DocType.HasValue) doc.DocType = dto.DocType.Value;
            if (dto.VerifyStatus.HasValue) doc.VerifyStatus = dto.VerifyStatus.Value;

            doc.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return _mapper.Map<EmployeeDocumentResponseDto>(doc);
        }

        public async Task<bool> DeleteEmployeeDocumentAsync(int id)
        {
            var doc = await _repository.GetByIdAsync(id);
            if (doc == null) return false;

            doc.IsDeleted = true;
            doc.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
