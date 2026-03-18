using System;
using System.Threading.Tasks;
using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Enums;
using TalentSphere.Services.Interfaces;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDTO dto)
        {
            var user = _mapper.Map<User>(dto);

            var added = await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();
            var userDetail = _mapper.Map<UserResponseDto>(added);
            return userDetail;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var users = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto> GetByIdDtoAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return null;
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserDTO dto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return null;

            // Use AutoMapper to apply only provided fields (mapping profile controls null/whitespace behavior)
            _mapper.Map(dto, user);

            user.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return false;

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
