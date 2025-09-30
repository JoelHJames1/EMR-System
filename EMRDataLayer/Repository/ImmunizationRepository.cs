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
    public class ImmunizationRepository : Repository<Immunization>, IImmunizationRepository
    {
        public ImmunizationRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Immunization>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Immunizations
                .Where(i => i.PatientId == patientId)
                .OrderByDescending(i => i.AdministeredDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Immunization>> GetImmunizationHistoryAsync(int patientId)
        {
            return await _context.Immunizations
                .Where(i => i.PatientId == patientId)
                .OrderBy(i => i.AdministeredDate)
                .ToListAsync();
        }
    }
}