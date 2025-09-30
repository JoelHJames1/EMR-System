using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IProcedureRepository : IRepository<Procedure>
    {
        Task<IEnumerable<Procedure>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Procedure>> GetByEncounterIdAsync(int encounterId);
        Task<IEnumerable<Procedure>> GetScheduledProceduresAsync();
    }
}