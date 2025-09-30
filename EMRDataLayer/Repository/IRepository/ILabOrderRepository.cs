using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface ILabOrderRepository : IRepository<LabOrder>
    {
        Task<IEnumerable<LabOrder>> GetByPatientIdAsync(int patientId);
        Task<LabOrder> GetByIdWithResultsAsync(int id);
        Task<IEnumerable<LabOrder>> GetPendingOrdersAsync();
    }
}