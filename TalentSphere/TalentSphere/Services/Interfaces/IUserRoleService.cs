using System.Threading.Tasks;
using System.Collections.Generic;
using TalentSphere.DTOs;
using TalentSphere.Models;

namespace TalentSphere.Services.Interfaces
{
    public interface IUserRoleService
    {
        Task<UserRoleResponseDto> CreateUserRoleAsync(CreateUserRoleDTO dto);
        Task<UserRoleResponseDto> GetByIdAsync(int id);
        Task<IEnumerable<UserRoleResponseDto>> GetAllAsync();
        Task<UserRoleResponseDto> UpdateUserRoleAsync(int id, CreateUserRoleDTO dto);
        Task<bool> DeleteUserRoleAsync(int id);
    }
}
