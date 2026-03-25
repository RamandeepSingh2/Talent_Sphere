using System.Threading.Tasks;
using System.Collections.Generic;
using TalentSphere.DTOs;
using TalentSphere.Models;

namespace TalentSphere.Services.Interfaces;

public interface ISelectionService
{
    Task<Selection> CreateSelectionAsync(CreateSelectionDTO dto);
    Task<Selection> GetByIdAsync(int id);
    Task<List<Selection>> GetAllSelectionsAsync();
    Task<Selection> UpdateSelectionAsync(int id, UpdateSelectionDTO dto);
    Task<bool> DeleteSelectionAsync(int id);
}
