using System.Threading.Tasks;
using TalentSphere.DTOs;
using TalentSphere.Models;

namespace TalentSphere.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> CreateUserAsync(CreateUserDTO dto);
        Task<User> GetByIdAsync(int id);
        Task<IEnumerable<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto> GetByIdDtoAsync(int id);
        Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserDTO dto);
        Task<bool> DeleteUserAsync(int id);
    }
}
