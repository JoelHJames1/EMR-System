using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class AllergyService : IAllergyService
    {
        private readonly IAllergyRepository _allergyRepository;
        private readonly ILogger<AllergyService> _logger;

        public AllergyService(IAllergyRepository allergyRepository, ILogger<AllergyService> logger)
        {
            _allergyRepository = allergyRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Allergy>> GetPatientAllergiesAsync(int patientId)
        {
            try
            {
                return await _allergyRepository.GetByPatientIdAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving allergies for patient {patientId}");
                throw;
            }
        }

        public async Task<IEnumerable<Allergy>> GetActiveAllergiesAsync(int patientId)
        {
            try
            {
                return await _allergyRepository.GetActiveAllergiesAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving active allergies for patient {patientId}");
                throw;
            }
        }

        public async Task<Allergy?> GetAllergyByIdAsync(int id)
        {
            try
            {
                return await _allergyRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving allergy {id}");
                throw;
            }
        }

        public async Task<Allergy> CreateAllergyAsync(Allergy allergy, string userId)
        {
            try
            {
                allergy.IsActive = true;
                allergy.OnsetDate = allergy.OnsetDate ?? DateTime.UtcNow;
                allergy.CreatedDate = DateTime.UtcNow;
                allergy.CreatedBy = userId;

                return await _allergyRepository.AddAsync(allergy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating allergy");
                throw;
            }
        }

        public async Task<Allergy> UpdateAllergyAsync(int id, Allergy allergy, string userId)
        {
            try
            {
                var existingAllergy = await _allergyRepository.GetByIdAsync(id);
                if (existingAllergy == null)
                {
                    throw new KeyNotFoundException($"Allergy with ID {id} not found");
                }

                allergy.Id = id;
                allergy.ModifiedDate = DateTime.UtcNow;
                allergy.ModifiedBy = userId;

                return await _allergyRepository.UpdateAsync(allergy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating allergy {id}");
                throw;
            }
        }

        public async Task<bool> DeleteAllergyAsync(int id)
        {
            try
            {
                var allergy = await _allergyRepository.GetByIdAsync(id);
                if (allergy == null)
                {
                    return false;
                }

                allergy.IsActive = false;
                await _allergyRepository.UpdateAsync(allergy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting allergy {id}");
                throw;
            }
        }

        public async Task<Allergy> AddAllergyAsync(Allergy allergy, string userId)
        {
            return await CreateAllergyAsync(allergy, userId);
        }

        public async Task<bool> DeactivateAllergyAsync(int id, string userId)
        {
            try
            {
                var allergy = await _allergyRepository.GetByIdAsync(id);
                if (allergy == null)
                {
                    return false;
                }

                allergy.IsActive = false;
                allergy.ModifiedDate = DateTime.UtcNow;
                allergy.ModifiedBy = userId;
                await _allergyRepository.UpdateAsync(allergy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating allergy {id}");
                throw;
            }
        }
    }
}