using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IEncounterService
    {
        Task<IEnumerable<Encounter>> GetAllEncountersAsync();
        Task<IEnumerable<Encounter>> GetEncountersAsync();
        Task<IEnumerable<Encounter>> GetEncountersAsync(int? patientId, string? status);
        Task<IEnumerable<Encounter>> GetPatientEncountersAsync(int patientId);
        Task<IEnumerable<Encounter>> GetActiveEncountersAsync();
        Task<Encounter?> GetEncounterByIdAsync(int id);
        Task<IEnumerable<object>> GetEncounterDiagnosesAsync(int encounterId);
        Task<Encounter> CreateEncounterAsync(Encounter encounter, string userId);
        Task<Encounter> UpdateEncounterAsync(int id, Encounter encounter, string userId);
        Task<bool> UpdateEncounterStatusAsync(int id, string status, string userId);
    }
}