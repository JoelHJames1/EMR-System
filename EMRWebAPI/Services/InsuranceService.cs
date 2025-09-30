using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class InsuranceService : IInsuranceService
    {
        private readonly IInsuranceRepository _insuranceRepository;
        private readonly ILogger<InsuranceService> _logger;

        public InsuranceService(IInsuranceRepository insuranceRepository, ILogger<InsuranceService> logger)
        {
            _insuranceRepository = insuranceRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Insurance>> GetPatientInsurancesAsync(int patientId)
        {
            try
            {
                return await _insuranceRepository.GetByPatientIdAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving insurances for patient {patientId}");
                throw;
            }
        }

        public async Task<Insurance?> GetActiveInsuranceAsync(int patientId)
        {
            try
            {
                return await _insuranceRepository.GetActiveInsuranceAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving active insurance for patient {patientId}");
                throw;
            }
        }

        public async Task<Insurance?> GetInsuranceByIdAsync(int id)
        {
            try
            {
                return await _insuranceRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving insurance {id}");
                throw;
            }
        }

        public async Task<Insurance> CreateInsuranceAsync(Insurance insurance, string userId)
        {
            try
            {
                insurance.IsActive = true;
                insurance.CreatedDate = DateTime.UtcNow;
                insurance.CreatedBy = userId;

                return await _insuranceRepository.AddAsync(insurance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating insurance");
                throw;
            }
        }

        public async Task<Insurance> UpdateInsuranceAsync(int id, Insurance insurance, string userId)
        {
            try
            {
                var existingInsurance = await _insuranceRepository.GetByIdAsync(id);
                if (existingInsurance == null)
                {
                    throw new KeyNotFoundException($"Insurance with ID {id} not found");
                }

                insurance.Id = id;
                insurance.ModifiedDate = DateTime.UtcNow;
                insurance.ModifiedBy = userId;

                return await _insuranceRepository.UpdateAsync(insurance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating insurance {id}");
                throw;
            }
        }

        public async Task<bool> DeleteInsuranceAsync(int id)
        {
            try
            {
                var insurance = await _insuranceRepository.GetByIdAsync(id);
                if (insurance == null)
                {
                    return false;
                }

                insurance.IsActive = false;
                await _insuranceRepository.UpdateAsync(insurance);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting insurance {id}");
                throw;
            }
        }

        public async Task<IEnumerable<Insurance>> GetPatientInsuranceAsync(int patientId)
        {
            return await GetPatientInsurancesAsync(patientId);
        }

        public async Task<Insurance> AddInsuranceAsync(Insurance insurance, string userId)
        {
            return await CreateInsuranceAsync(insurance, userId);
        }

        public async Task<bool> VerifyInsuranceAsync(int id)
        {
            try
            {
                var insurance = await _insuranceRepository.GetByIdAsync(id);
                if (insurance == null)
                {
                    return false;
                }

                insurance.IsActive = true;
                insurance.ModifiedDate = DateTime.UtcNow;
                await _insuranceRepository.UpdateAsync(insurance);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error verifying insurance {id}");
                throw;
            }
        }

        public async Task<bool> DeactivateInsuranceAsync(int id, string userId)
        {
            try
            {
                var insurance = await _insuranceRepository.GetByIdAsync(id);
                if (insurance == null)
                {
                    return false;
                }

                insurance.IsActive = false;
                insurance.ModifiedDate = DateTime.UtcNow;
                insurance.ModifiedBy = userId;
                await _insuranceRepository.UpdateAsync(insurance);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating insurance {id}");
                throw;
            }
        }
    }
}