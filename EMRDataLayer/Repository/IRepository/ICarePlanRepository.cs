using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface ICarePlanRepository : IRepository<CarePlan>
    {
        Task<IEnumerable<CarePlan>> GetByPatientIdAsync(int patientId);
        Task<CarePlan> GetByIdWithActivitiesAsync(int id);
        Task<IEnumerable<CarePlan>> GetActiveCarePlansAsync(int patientId);
    }
}