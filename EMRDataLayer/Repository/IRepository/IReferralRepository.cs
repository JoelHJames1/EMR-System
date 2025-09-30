using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IReferralRepository : IRepository<Referral>
    {
        Task<IEnumerable<Referral>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Referral>> GetPendingReferralsAsync();
        Task<IEnumerable<Referral>> GetByReferringProviderIdAsync(int providerId);
    }
}