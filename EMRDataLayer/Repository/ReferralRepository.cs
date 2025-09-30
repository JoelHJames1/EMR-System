using EMRDataLayer.DataContext;
using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository
{
    public class ReferralRepository : Repository<Referral>, IReferralRepository
    {
        public ReferralRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Referral>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Referrals
                .Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.ReferralDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Referral>> GetPendingReferralsAsync()
        {
            return await _context.Referrals
                .Where(r => r.Status == "Pending")
                .OrderBy(r => r.ReferralDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Referral>> GetByReferringProviderIdAsync(int providerId)
        {
            return await _context.Referrals
                .Where(r => r.ReferringProviderId == providerId)
                .OrderByDescending(r => r.ReferralDate)
                .ToListAsync();
        }
    }
}