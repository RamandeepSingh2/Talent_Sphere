using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentSphere.Config;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Models;

namespace TalentSphere.Repositories
{
    public class EmployeeDocumentRepository : IEmployeeDocumentRepository
    {
        private readonly AppDbContext _context;

        public EmployeeDocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeDocument> AddAsync(EmployeeDocument doc)
        {
            await _context.EmployeeDocs.AddAsync(doc);
            await _context.SaveChangesAsync();
            return doc;
        }

        public async Task<EmployeeDocument> GetByIdAsync(int id)
        {
            return await _context.Set<EmployeeDocument>().FirstOrDefaultAsync(d => d.DocumentID == id && !EF.Property<bool>(d, "IsDeleted"));
        }

        public async Task SaveChangesAsync()
        {
                await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EmployeeDocument>> GetAllAsync()
        {
            return await _context.Set<EmployeeDocument>()
                .AsNoTracking()
                .Where(d => !EF.Property<bool>(d, "IsDeleted"))
                .ToListAsync();
        }

        public async Task<bool> EmployeeExistsAsync(int employeeId)
        {
            return await _context.Set<Employee>().AnyAsync(e => e.EmployeeID == employeeId);
        }
    }
}
