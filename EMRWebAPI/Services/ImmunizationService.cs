using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class ImmunizationService : IImmunizationService
    {
        private readonly IImmunizationRepository _immunizationRepository;
        private readonly ILogger<ImmunizationService> _logger;

        public ImmunizationService(IImmunizationRepository immunizationRepository, ILogger<ImmunizationService> logger)
        {
            _immunizationRepository = immunizationRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Immunization>> GetPatientImmunizationsAsync(int patientId)
        {
            try
            {
                return await _immunizationRepository.GetByPatientIdAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving immunizations for patient {patientId}");
                throw;
            }
        }

        public async Task<IEnumerable<Immunization>> GetImmunizationHistoryAsync(int patientId)
        {
            try
            {
                return await _immunizationRepository.GetImmunizationHistoryAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving immunization history for patient {patientId}");
                throw;
            }
        }

        public async Task<Immunization?> GetImmunizationByIdAsync(int id)
        {
            try
            {
                return await _immunizationRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving immunization {id}");
                throw;
            }
        }

        public async Task<Immunization> CreateImmunizationAsync(Immunization immunization, string userId)
        {
            try
            {
                if (immunization.AdministeredDate == default(DateTime))
                {
                    immunization.AdministeredDate = DateTime.UtcNow;
                }
                immunization.CreatedDate = DateTime.UtcNow;
                immunization.CreatedBy = userId;

                return await _immunizationRepository.AddAsync(immunization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating immunization");
                throw;
            }
        }

        public async Task<Immunization> UpdateImmunizationAsync(int id, Immunization immunization, string userId)
        {
            try
            {
                var existingImmunization = await _immunizationRepository.GetByIdAsync(id);
                if (existingImmunization == null)
                {
                    throw new KeyNotFoundException($"Immunization with ID {id} not found");
                }

                immunization.Id = id;
                immunization.ModifiedDate = DateTime.UtcNow;
                immunization.ModifiedBy = userId;

                return await _immunizationRepository.UpdateAsync(immunization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating immunization {id}");
                throw;
            }
        }

        public async Task<bool> DeleteImmunizationAsync(int id)
        {
            try
            {
                var immunization = await _immunizationRepository.GetByIdAsync(id);
                if (immunization == null)
                {
                    return false;
                }

                await _immunizationRepository.DeleteAsync(immunization);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting immunization {id}");
                throw;
            }
        }

        public async Task<Immunization> RecordImmunizationAsync(Immunization immunization, string userId)
        {
            return await CreateImmunizationAsync(immunization, userId);
        }
    }
}