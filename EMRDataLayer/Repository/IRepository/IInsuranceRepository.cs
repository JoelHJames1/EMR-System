using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IInsuranceRepository : IRepository<Insurance>
    {
        Task<IEnumerable<Insurance>> GetByPatientIdAsync(int patientId);
        Task<Insurance> GetActiveInsuranceAsync(int patientId);
    }
}