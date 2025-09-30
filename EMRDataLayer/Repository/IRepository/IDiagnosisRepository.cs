using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IDiagnosisRepository : IRepository<Diagnosis>
    {
        Task<IEnumerable<Diagnosis>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Diagnosis>> GetByEncounterIdAsync(int encounterId);
        Task<IEnumerable<Diagnosis>> GetActiveDiagnosesAsync(int patientId);
    }
}