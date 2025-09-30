using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IMedicationService
    {
        Task<IEnumerable<Medication>> GetAllMedicationsAsync();
        Task<IEnumerable<Medication>> SearchMedicationsAsync(string searchTerm);
        Task<IEnumerable<Medication>> GetControlledSubstancesAsync();
        Task<Medication?> GetMedicationByIdAsync(int id);
        Task<Medication?> GetByNDCAsync(string ndc);
        Task<Medication> CreateMedicationAsync(Medication medication, string userId);
        Task<Medication> UpdateMedicationAsync(int id, Medication medication, string userId);
        Task<bool> DeleteMedicationAsync(int id);
    }
}