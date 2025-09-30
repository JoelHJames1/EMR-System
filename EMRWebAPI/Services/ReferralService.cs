using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class ReferralService : IReferralService
    {
        private readonly IReferralRepository _referralRepository;
        private readonly ILogger<ReferralService> _logger;

        public ReferralService(IReferralRepository referralRepository, ILogger<ReferralService> logger)
        {
            _referralRepository = referralRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Referral>> GetPatientReferralsAsync(int patientId)
        {
            try
            {
                return await _referralRepository.GetByPatientIdAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving referrals for patient {patientId}");
                throw;
            }
        }

        public async Task<IEnumerable<Referral>> GetPendingReferralsAsync()
        {
            try
            {
                return await _referralRepository.GetPendingReferralsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending referrals");
                throw;
            }
        }

        public async Task<IEnumerable<Referral>> GetByReferringProviderAsync(int providerId)
        {
            try
            {
                return await _referralRepository.GetByReferringProviderIdAsync(providerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving referrals by provider {providerId}");
                throw;
            }
        }

        public async Task<Referral?> GetReferralByIdAsync(int id)
        {
            try
            {
                return await _referralRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving referral {id}");
                throw;
            }
        }

        public async Task<Referral> CreateReferralAsync(Referral referral, string userId)
        {
            try
            {
                referral.Status = "Pending";
                referral.ReferralDate = DateTime.UtcNow;
                referral.CreatedDate = DateTime.UtcNow;
                referral.CreatedBy = userId;

                return await _referralRepository.AddAsync(referral);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating referral");
                throw;
            }
        }

        public async Task<bool> UpdateReferralStatusAsync(int id, string status, string userId)
        {
            try
            {
                var referral = await _referralRepository.GetByIdAsync(id);
                if (referral == null)
                {
                    return false;
                }

                referral.Status = status;
                referral.ModifiedDate = DateTime.UtcNow;
                referral.ModifiedBy = userId;

                await _referralRepository.UpdateAsync(referral);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating referral status {id}");
                throw;
            }
        }
    }
}