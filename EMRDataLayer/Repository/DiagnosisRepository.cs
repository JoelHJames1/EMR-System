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
    public class DiagnosisRepository : Repository<Diagnosis>, IDiagnosisRepository
    {
        public DiagnosisRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Diagnosis>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Diagnoses
                .Where(d => d.PatientId == patientId)
                .OrderByDescending(d => d.DiagnosisDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Diagnosis>> GetByEncounterIdAsync(int encounterId)
        {
            return await _context.Diagnoses
                .Where(d => d.EncounterId == encounterId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Diagnosis>> GetActiveDiagnosesAsync(int patientId)
        {
            return await _context.Diagnoses
                .Where(d => d.PatientId == patientId && d.ClinicalStatus == "Active")
                .ToListAsync();
        }
    }
}