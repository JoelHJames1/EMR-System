using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly ILogger<PrescriptionService> _logger;

        public PrescriptionService(IPrescriptionRepository prescriptionRepository, ILogger<PrescriptionService> logger)
        {
            _prescriptionRepository = prescriptionRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Prescription>> GetPatientPrescriptionsAsync(int patientId)
        {
            try
            {
                return await _prescriptionRepository.GetByPatientIdAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving prescriptions for patient {patientId}");
                throw;
            }
        }

        public async Task<IEnumerable<Prescription>> GetActivePrescriptionsAsync(int patientId)
        {
            try
            {
                return await _prescriptionRepository.GetActivePrescriptionsAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving active prescriptions for patient {patientId}");
                throw;
            }
        }

        public async Task<Prescription?> GetPrescriptionByIdAsync(int id)
        {
            try
            {
                return await _prescriptionRepository.GetByIdWithMedicationAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving prescription {id}");
                throw;
            }
        }

        public async Task<Prescription> CreatePrescriptionAsync(Prescription prescription, string userId)
        {
            try
            {
                prescription.Status = "Active";
                prescription.StartDate = DateTime.UtcNow;
                prescription.CreatedDate = DateTime.UtcNow;
                prescription.CreatedBy = userId;

                return await _prescriptionRepository.AddAsync(prescription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating prescription");
                throw;
            }
        }

        public async Task<Prescription> UpdatePrescriptionAsync(int id, Prescription prescription, string userId)
        {
            try
            {
                var existingPrescription = await _prescriptionRepository.GetByIdAsync(id);
                if (existingPrescription == null)
                {
                    throw new KeyNotFoundException($"Prescription with ID {id} not found");
                }

                prescription.Id = id;
                prescription.ModifiedDate = DateTime.UtcNow;
                prescription.ModifiedBy = userId;

                return await _prescriptionRepository.UpdateAsync(prescription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating prescription {id}");
                throw;
            }
        }

        public async Task<bool> RefillPrescriptionAsync(int id, string userId)
        {
            try
            {
                var prescription = await _prescriptionRepository.GetByIdAsync(id);
                if (prescription == null)
                {
                    return false;
                }

                if (prescription.Refills > 0)
                {
                    prescription.Refills--;
                    prescription.ModifiedDate = DateTime.UtcNow;
                    prescription.ModifiedBy = userId;

                    await _prescriptionRepository.UpdateAsync(prescription);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error refilling prescription {id}");
                throw;
            }
        }

        public async Task<bool> DiscontinuePrescriptionAsync(int id, string userId)
        {
            try
            {
                var prescription = await _prescriptionRepository.GetByIdAsync(id);
                if (prescription == null)
                {
                    return false;
                }

                prescription.Status = "Discontinued";
                prescription.EndDate = DateTime.UtcNow;
                prescription.ModifiedDate = DateTime.UtcNow;
                prescription.ModifiedBy = userId;

                await _prescriptionRepository.UpdateAsync(prescription);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error discontinuing prescription {id}");
                throw;
            }
        }
    }
}