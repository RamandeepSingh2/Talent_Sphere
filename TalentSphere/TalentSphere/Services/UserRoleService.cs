using System;
using System.Threading.Tasks;
using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Services.Interfaces;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _repository;
        private readonly IMapper _mapper;

        public UserRoleService(IUserRoleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserRoleResponseDto> CreateUserRoleAsync(CreateUserRoleDTO dto)
        {
            var userRole = _mapper.Map<UserRole>(dto);
            userRole.CreatedAt = DateTime.UtcNow;

            var added = await _repository.AddAsync(userRole);
            await _repository.SaveChangesAsync();
            return _mapper.Map<UserRoleResponseDto>(added);
        }

        public async Task<UserRoleResponseDto> GetByIdAsync(int id)
        {
            var userRole = await _repository.GetByIdAsync(id);
            if (userRole == null) return null;
            return _mapper.Map<UserRoleResponseDto>(userRole);
        }
        
        public async Task<IEnumerable<UserRoleResponseDto>> GetAllAsync()
        {
            var userRoles = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserRoleResponseDto>>(userRoles);
        }

        public async Task<UserRoleResponseDto> UpdateUserRoleAsync(int id, CreateUserRoleDTO dto)
        {
            var userRole = await _repository.GetByIdAsync(id);
            if (userRole == null) return null;

            // Apply requested changes
            userRole.UserId = dto.UserId;
            userRole.RoleId = dto.RoleId;
            userRole.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();
            return _mapper.Map<UserRoleResponseDto>(userRole);
        }

        public async Task<bool> DeleteUserRoleAsync(int id)
        {
            var userRole = await _repository.GetByIdAsync(id);
            if (userRole == null) return false;

            userRole.IsDeleted = true;
            userRole.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
