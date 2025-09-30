using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IPrescriptionRepository : IRepository<Prescription>
    {
        Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Prescription>> GetActivePrescriptionsAsync(int patientId);
        Task<Prescription> GetByIdWithMedicationAsync(int id);
    }
}