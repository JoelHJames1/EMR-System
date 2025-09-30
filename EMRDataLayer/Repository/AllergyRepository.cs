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
    public class AllergyRepository : Repository<Allergy>, IAllergyRepository
    {
        public AllergyRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Allergy>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Allergies
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.OnsetDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Allergy>> GetActiveAllergiesAsync(int patientId)
        {
            return await _context.Allergies
                .Where(a => a.PatientId == patientId && a.IsActive)
                .ToListAsync();
        }
    }
}