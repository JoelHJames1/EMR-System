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
    public class PrescriptionRepository : Repository<Prescription>, IPrescriptionRepository
    {
        public PrescriptionRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Prescriptions
                .Include(p => p.Medication)
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetActivePrescriptionsAsync(int patientId)
        {
            return await _context.Prescriptions
                .Include(p => p.Medication)
                .Where(p => p.PatientId == patientId && p.Status == "Active")
                .ToListAsync();
        }

        public async Task<Prescription> GetByIdWithMedicationAsync(int id)
        {
            return await _context.Prescriptions
                .Include(p => p.Medication)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}