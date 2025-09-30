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
    public class EncounterRepository : Repository<Encounter>, IEncounterRepository
    {
        public EncounterRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Encounter>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Encounters
                .Where(e => e.PatientId == patientId)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<Encounter> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Encounters
                .Include(e => e.Diagnoses)
                .Include(e => e.Procedures)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Encounter>> GetActiveEncountersAsync()
        {
            return await _context.Encounters
                .Where(e => e.Status == "InProgress")
                .ToListAsync();
        }
    }
}