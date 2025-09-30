using EMRDataLayer.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IEncounterRepository : IRepository<Encounter>
    {
        Task<IEnumerable<Encounter>> GetByPatientIdAsync(int patientId);
        Task<Encounter> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Encounter>> GetActiveEncountersAsync();
    }
}