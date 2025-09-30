using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface ILabResultRepository : IRepository<LabResult>
    {
        Task<IEnumerable<LabResult>> GetByLabOrderIdAsync(int labOrderId);
        Task<IEnumerable<LabResult>> GetByPatientIdAsync(int patientId);
    }
}