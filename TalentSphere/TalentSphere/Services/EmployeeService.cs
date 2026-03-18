using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Enums;
using TalentSphere.Services.Interfaces;
using TalentSphere.Repositories.Interfaces;

namespace TalentSphere.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDTO dto)
        {
            var employee = _mapper.Map<Employee>(dto);
            employee.CreatedAt = DateTime.UtcNow;

            if (employee.Status == 0)
            {
                employee.Status = EmployeeStatus.Active;
            }

            var added = await _repository.AddAsync(employee);
            await _repository.SaveChangesAsync();
            return _mapper.Map<EmployeeResponseDto>(added);
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetAllAsync()
        {
            var employees = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees);
        }

        public async Task<EmployeeResponseDto> GetByIdDtoAsync(int id)
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null) return null;
            return _mapper.Map<EmployeeResponseDto>(employee);
        }

        public async Task<EmployeeResponseDto> UpdateEmployeeAsync(int id, UpdateEmployeeDTO dto)
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null) return null;

            // Apply partial update via AutoMapper mapping profile (ignore null/whitespace)
            _mapper.Map(dto, employee);
            employee.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();

            return _mapper.Map<EmployeeResponseDto>(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null) return false;

            employee.IsDeleted = true;
            employee.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
