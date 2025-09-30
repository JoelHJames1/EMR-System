using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IImmunizationService
    {
        Task<IEnumerable<Immunization>> GetPatientImmunizationsAsync(int patientId);
        Task<IEnumerable<Immunization>> GetImmunizationHistoryAsync(int patientId);
        Task<Immunization?> GetImmunizationByIdAsync(int id);
        Task<Immunization> CreateImmunizationAsync(Immunization immunization, string userId);
        Task<Immunization> UpdateImmunizationAsync(int id, Immunization immunization, string userId);
        Task<bool> DeleteImmunizationAsync(int id);
    }
}