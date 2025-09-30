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
    public class LabResultRepository : Repository<LabResult>, ILabResultRepository
    {
        public LabResultRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<LabResult>> GetByLabOrderIdAsync(int labOrderId)
        {
            return await _context.LabResults
                .Where(r => r.LabOrderId == labOrderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<LabResult>> GetByPatientIdAsync(int patientId)
        {
            return await _context.LabResults
                .Include(r => r.LabOrder)
                .Where(r => r.LabOrder.PatientId == patientId)
                .OrderByDescending(r => r.LabOrder.OrderedDate)
                .ToListAsync();
        }
    }
}