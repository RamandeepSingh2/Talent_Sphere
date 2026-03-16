using System.Threading.Tasks;
using TalentSphere.DTOs;
using TalentSphere.Models;

namespace TalentSphere.Services
{
    public interface ISelectionService
    {
        Task<Selection> CreateSelectionAsync(CreateSelectionDTO dto);
        Task<Selection> GetByIdAsync(int id);
    }
}
