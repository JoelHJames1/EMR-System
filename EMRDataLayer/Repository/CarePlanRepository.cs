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
    public class CarePlanRepository : Repository<CarePlan>, ICarePlanRepository
    {
        public CarePlanRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<CarePlan>> GetByPatientIdAsync(int patientId)
        {
            return await _context.CarePlans
                .Where(c => c.PatientId == patientId)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<CarePlan> GetByIdWithActivitiesAsync(int id)
        {
            return await _context.CarePlans
                .Include(c => c.Activities)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CarePlan>> GetActiveCarePlansAsync(int patientId)
        {
            return await _context.CarePlans
                .Where(c => c.PatientId == patientId && c.Status == "Active")
                .ToListAsync();
        }
    }
}