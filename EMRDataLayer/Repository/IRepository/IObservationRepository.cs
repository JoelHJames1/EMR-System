using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IObservationRepository : IRepository<Observation>
    {
        Task<IEnumerable<Observation>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Observation>> GetByEncounterIdAsync(int encounterId);
        Task<Observation> GetLatestVitalsAsync(int patientId);
    }
}