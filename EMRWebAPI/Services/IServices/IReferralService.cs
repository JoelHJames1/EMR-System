using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IReferralService
    {
        Task<IEnumerable<Referral>> GetPatientReferralsAsync(int patientId);
        Task<IEnumerable<Referral>> GetPendingReferralsAsync();
        Task<IEnumerable<Referral>> GetPendingReferralsAsync(int? providerId);
        Task<IEnumerable<Referral>> GetByReferringProviderAsync(int providerId);
        Task<Referral?> GetReferralByIdAsync(int id);
        Task<Referral> CreateReferralAsync(Referral referral, string userId);
        Task<bool> UpdateReferralStatusAsync(int id, string status, string userId);
    }
}