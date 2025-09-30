using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IAllergyService
    {
        Task<IEnumerable<Allergy>> GetPatientAllergiesAsync(int patientId);
        Task<IEnumerable<Allergy>> GetActiveAllergiesAsync(int patientId);
        Task<Allergy?> GetAllergyByIdAsync(int id);
        Task<Allergy> CreateAllergyAsync(Allergy allergy, string userId);
        Task<Allergy> UpdateAllergyAsync(int id, Allergy allergy, string userId);
        Task<bool> DeleteAllergyAsync(int id);
    }
}