using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IClinicalNoteRepository : IRepository<ClinicalNote>
    {
        Task<IEnumerable<ClinicalNote>> GetByEncounterIdAsync(int encounterId);
        Task<IEnumerable<ClinicalNote>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<ClinicalNote>> GetUnsignedNotesAsync(int providerId);
    }
}