using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IAllergyRepository : IRepository<Allergy>
    {
        Task<IEnumerable<Allergy>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Allergy>> GetActiveAllergiesAsync(int patientId);
    }
}