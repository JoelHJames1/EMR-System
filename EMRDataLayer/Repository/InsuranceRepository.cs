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
    public class InsuranceRepository : Repository<Insurance>, IInsuranceRepository
    {
        public InsuranceRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Insurance>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Insurances
                .Where(i => i.PatientId == patientId)
                .OrderByDescending(i => i.EffectiveDate)
                .ToListAsync();
        }

        public async Task<Insurance> GetActiveInsuranceAsync(int patientId)
        {
            return await _context.Insurances
                .Where(i => i.PatientId == patientId &&
                           i.IsActive &&
                           (i.ExpirationDate == null || i.ExpirationDate > DateTime.Now))
                .FirstOrDefaultAsync();
        }
    }
}