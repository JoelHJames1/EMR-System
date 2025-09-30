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
    public class ProcedureRepository : Repository<Procedure>, IProcedureRepository
    {
        public ProcedureRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Procedure>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Procedures
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.PerformedDate ?? p.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Procedure>> GetByEncounterIdAsync(int encounterId)
        {
            return await _context.Procedures
                .Where(p => p.EncounterId == encounterId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Procedure>> GetScheduledProceduresAsync()
        {
            return await _context.Procedures
                .Where(p => p.Status == "Scheduled" && p.ScheduledDate >= DateTime.Now)
                .OrderBy(p => p.ScheduledDate)
                .ToListAsync();
        }
    }
}