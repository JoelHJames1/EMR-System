using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IDiagnosisService
    {
        Task<IEnumerable<Diagnosis>> GetPatientDiagnosesAsync(int patientId);
        Task<IEnumerable<Diagnosis>> GetByEncounterIdAsync(int encounterId);
        Task<IEnumerable<Diagnosis>> GetActiveDiagnosesAsync(int patientId);
        Task<Diagnosis> CreateDiagnosisAsync(Diagnosis diagnosis, string userId);
        Task<Diagnosis> UpdateDiagnosisAsync(int id, Diagnosis diagnosis, string userId);
    }
}