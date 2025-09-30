using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IInsuranceService
    {
        Task<IEnumerable<Insurance>> GetPatientInsurancesAsync(int patientId);
        Task<IEnumerable<Insurance>> GetPatientInsuranceAsync(int patientId);
        Task<Insurance?> GetActiveInsuranceAsync(int patientId);
        Task<Insurance?> GetInsuranceByIdAsync(int id);
        Task<Insurance> CreateInsuranceAsync(Insurance insurance, string userId);
        Task<Insurance> AddInsuranceAsync(Insurance insurance, string userId);
        Task<Insurance> UpdateInsuranceAsync(int id, Insurance insurance, string userId);
        Task<bool> VerifyInsuranceAsync(int id);
        Task<bool> DeleteInsuranceAsync(int id);
        Task<bool> DeactivateInsuranceAsync(int id, string userId);
    }
}