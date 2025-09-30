using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IInsuranceService
    {
        Task<IEnumerable<Insurance>> GetPatientInsurancesAsync(int patientId);
        Task<Insurance?> GetActiveInsuranceAsync(int patientId);
        Task<Insurance?> GetInsuranceByIdAsync(int id);
        Task<Insurance> CreateInsuranceAsync(Insurance insurance, string userId);
        Task<Insurance> UpdateInsuranceAsync(int id, Insurance insurance, string userId);
        Task<bool> DeleteInsuranceAsync(int id);
    }
}