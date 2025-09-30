using EMRDataLayer.DataContext;
using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(EMRDBContext context) : base(context) { }

        public async Task<Patient> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm)
        {
            return await _context.Patients
                .Where(p => p.FirstName.Contains(searchTerm) ||
                           p.LastName.Contains(searchTerm) ||
                           p.Email.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetActivePatients()
        {
            return await _context.Patients
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<bool> PatientExists(string mrn)
        {
            return await _context.Patients.AnyAsync(p => p.Email == mrn);
        }

        public async Task<IEnumerable<Patient>> GetAllWithPaginationAsync(int page, int pageSize, string? search)
        {
            var query = _context.Patients.Include(p => p.Address).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.FirstName.Contains(search) ||
                                        p.LastName.Contains(search) ||
                                        p.Email.Contains(search));
            }

            return await query
                .Where(p => p.IsActive)
                .OrderBy(p => p.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? search)
        {
            var query = _context.Patients.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.FirstName.Contains(search) ||
                                        p.LastName.Contains(search) ||
                                        p.Email.Contains(search));
            }

            return await query.Where(p => p.IsActive).CountAsync();
        }
    }
}