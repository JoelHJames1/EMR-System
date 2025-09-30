using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IImmunizationRepository : IRepository<Immunization>
    {
        Task<IEnumerable<Immunization>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Immunization>> GetImmunizationHistoryAsync(int patientId);
    }
}