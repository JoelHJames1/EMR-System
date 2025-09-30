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
    public class ObservationRepository : Repository<Observation>, IObservationRepository
    {
        public ObservationRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Observation>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Observations
                .Where(o => o.PatientId == patientId)
                .OrderByDescending(o => o.ObservationDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Observation>> GetByEncounterIdAsync(int encounterId)
        {
            return await _context.Observations
                .Where(o => o.EncounterId == encounterId)
                .OrderByDescending(o => o.ObservationDateTime)
                .ToListAsync();
        }

        public async Task<Observation> GetLatestVitalsAsync(int patientId)
        {
            return await _context.Observations
                .Where(o => o.PatientId == patientId && o.ObservationType == "Vital Signs")
                .OrderByDescending(o => o.ObservationDateTime)
                .FirstOrDefaultAsync();
        }
    }
}