using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly IMedicationRepository _medicationRepository;
        private readonly ILogger<MedicationService> _logger;

        public MedicationService(IMedicationRepository medicationRepository, ILogger<MedicationService> logger)
        {
            _medicationRepository = medicationRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Medication>> GetAllMedicationsAsync()
        {
            try
            {
                return await _medicationRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all medications");
                throw;
            }
        }

        public async Task<IEnumerable<Medication>> SearchMedicationsAsync(string searchTerm)
        {
            try
            {
                return await _medicationRepository.SearchMedicationsAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching medications with term: {searchTerm}");
                throw;
            }
        }

        public async Task<IEnumerable<Medication>> GetControlledSubstancesAsync()
        {
            try
            {
                return await _medicationRepository.GetControlledSubstancesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving controlled substances");
                throw;
            }
        }

        public async Task<Medication?> GetMedicationByIdAsync(int id)
        {
            try
            {
                return await _medicationRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving medication {id}");
                throw;
            }
        }

        public async Task<Medication?> GetByNDCAsync(string ndc)
        {
            try
            {
                return await _medicationRepository.GetByNDCAsync(ndc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving medication by NDC {ndc}");
                throw;
            }
        }

        public async Task<Medication> CreateMedicationAsync(Medication medication, string userId)
        {
            try
            {
                medication.IsActive = true;
                medication.CreatedDate = DateTime.UtcNow;
                medication.CreatedBy = userId;

                return await _medicationRepository.AddAsync(medication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medication");
                throw;
            }
        }

        public async Task<Medication> UpdateMedicationAsync(int id, Medication medication, string userId)
        {
            try
            {
                var existingMedication = await _medicationRepository.GetByIdAsync(id);
                if (existingMedication == null)
                {
                    throw new KeyNotFoundException($"Medication with ID {id} not found");
                }

                medication.Id = id;
                medication.ModifiedDate = DateTime.UtcNow;
                medication.ModifiedBy = userId;

                return await _medicationRepository.UpdateAsync(medication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating medication {id}");
                throw;
            }
        }

        public async Task<bool> DeleteMedicationAsync(int id)
        {
            try
            {
                var medication = await _medicationRepository.GetByIdAsync(id);
                if (medication == null)
                {
                    return false;
                }

                medication.IsActive = false;
                await _medicationRepository.UpdateAsync(medication);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting medication {id}");
                throw;
            }
        }
    }
}