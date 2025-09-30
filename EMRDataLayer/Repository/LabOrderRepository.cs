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
    public class LabOrderRepository : Repository<LabOrder>, ILabOrderRepository
    {
        public LabOrderRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<LabOrder>> GetByPatientIdAsync(int patientId)
        {
            return await _context.LabOrders
                .Where(l => l.PatientId == patientId)
                .OrderByDescending(l => l.OrderedDate)
                .ToListAsync();
        }

        public async Task<LabOrder> GetByIdWithResultsAsync(int id)
        {
            return await _context.LabOrders
                .Include(l => l.LabResults)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<LabOrder>> GetPendingOrdersAsync()
        {
            return await _context.LabOrders
                .Where(l => l.Status == "Ordered")
                .OrderBy(l => l.OrderedDate)
                .ToListAsync();
        }
    }
}