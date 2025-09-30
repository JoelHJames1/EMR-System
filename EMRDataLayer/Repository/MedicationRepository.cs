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
    public class MedicationRepository : Repository<Medication>, IMedicationRepository
    {
        public MedicationRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Medication>> SearchMedicationsAsync(string searchTerm)
        {
            return await _context.Medications
                .Where(m => m.GenericName.Contains(searchTerm) ||
                           m.BrandName.Contains(searchTerm) ||
                           (m.NDC != null && m.NDC.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<Medication> GetByNDCAsync(string ndc)
        {
            return await _context.Medications
                .FirstOrDefaultAsync(m => m.NDC == ndc);
        }

        public async Task<IEnumerable<Medication>> GetControlledSubstancesAsync()
        {
            return await _context.Medications
                .Where(m => m.DEASchedule != null && m.DEASchedule != "None")
                .OrderBy(m => m.DEASchedule)
                .ToListAsync();
        }
    }
}