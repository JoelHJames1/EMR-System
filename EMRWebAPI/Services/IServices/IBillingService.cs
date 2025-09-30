using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IBillingService
    {
        Task<IEnumerable<Billing>> GetAllBillingsAsync();
        Task<IEnumerable<Billing>> GetPatientBillingsAsync(int patientId);
        Task<IEnumerable<Billing>> GetUnpaidInvoicesAsync();
        Task<Billing?> GetBillingByIdAsync(int id);
        Task<decimal> GetPatientBalanceAsync(int patientId);
        Task<Billing> CreateBillingAsync(Billing billing, string userId);
        Task<Billing> UpdateBillingAsync(int id, Billing billing, string userId);
        Task<bool> RecordPaymentAsync(int id, decimal amount, string userId);
    }
}