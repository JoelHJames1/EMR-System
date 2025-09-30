using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IBillingRepository : IRepository<Billing>
    {
        Task<IEnumerable<Billing>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Billing>> GetUnpaidInvoicesAsync();
        Task<decimal> GetPatientBalanceAsync(int patientId);
    }
}