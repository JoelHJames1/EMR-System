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
    public class ClinicalNoteRepository : Repository<ClinicalNote>, IClinicalNoteRepository
    {
        public ClinicalNoteRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<ClinicalNote>> GetByEncounterIdAsync(int encounterId)
        {
            return await _context.ClinicalNotes
                .Where(n => n.EncounterId == encounterId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClinicalNote>> GetByPatientIdAsync(int patientId)
        {
            return await _context.ClinicalNotes
                .Include(n => n.Encounter)
                .Where(n => n.Encounter.PatientId == patientId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClinicalNote>> GetUnsignedNotesAsync(int providerId)
        {
            return await _context.ClinicalNotes
                .Where(n => n.ProviderId == providerId && !n.IsSigned)
                .OrderBy(n => n.CreatedDate)
                .ToListAsync();
        }
    }
}