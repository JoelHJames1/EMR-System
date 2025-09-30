using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IObservationService
    {
        Task<IEnumerable<Observation>> GetPatientObservationsAsync(int patientId);
        Task<IEnumerable<Observation>> GetByEncounterIdAsync(int encounterId);
        Task<Observation?> GetLatestVitalsAsync(int patientId);
        Task<Observation?> GetObservationByIdAsync(int id);
        Task<Observation> CreateObservationAsync(Observation observation, string userId);
        Task<Observation> UpdateObservationAsync(int id, Observation observation, string userId);
    }
}