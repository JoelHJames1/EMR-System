using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IMedicationRepository : IRepository<Medication>
    {
        Task<IEnumerable<Medication>> SearchMedicationsAsync(string searchTerm);
        Task<Medication> GetByNDCAsync(string ndc);
        Task<IEnumerable<Medication>> GetControlledSubstancesAsync();
    }
}