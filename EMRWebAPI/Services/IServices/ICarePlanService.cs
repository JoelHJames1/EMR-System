using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface ICarePlanService
    {
        Task<IEnumerable<CarePlan>> GetPatientCarePlansAsync(int patientId);
        Task<IEnumerable<CarePlan>> GetActiveCarePlansAsync(int patientId);
        Task<CarePlan?> GetCarePlanByIdAsync(int id);
        Task<CarePlan> CreateCarePlanAsync(CarePlan carePlan, string userId);
        Task<CarePlanActivity> AddActivityAsync(int carePlanId, CarePlanActivity activity);
        Task<bool> UpdateActivityStatusAsync(int activityId, string status);
        Task<bool> UpdateCarePlanStatusAsync(int id, string status, string userId);
    }
}