using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<PatientService> _logger;

        public PatientService(IPatientRepository patientRepository, ILogger<PatientService> logger)
        {
            _patientRepository = patientRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync(int page, int pageSize, string? search)
        {
            try
            {
                return await _patientRepository.GetAllWithPaginationAsync(page, pageSize, search);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patients");
                throw;
            }
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            try
            {
                return await _patientRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving patient {id}");
                throw;
            }
        }

        public async Task<Patient> CreatePatientAsync(Patient patient)
        {
            try
            {
                patient.CreatedDate = DateTime.UtcNow;
                patient.IsActive = true;
                return await _patientRepository.AddAsync(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient");
                throw;
            }
        }

        public async Task<Patient> UpdatePatientAsync(int id, Patient patient)
        {
            try
            {
                var existingPatient = await _patientRepository.GetByIdAsync(id);
                if (existingPatient == null)
                {
                    throw new KeyNotFoundException($"Patient with ID {id} not found");
                }

                patient.Id = id;
                patient.ModifiedDate = DateTime.UtcNow;
                return await _patientRepository.UpdateAsync(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating patient {id}");
                throw;
            }
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            try
            {
                var patient = await _patientRepository.GetByIdAsync(id);
                if (patient == null)
                {
                    return false;
                }

                patient.IsActive = false;
                await _patientRepository.UpdateAsync(patient);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting patient {id}");
                throw;
            }
        }

        public async Task<int> GetTotalPatientsCountAsync(string? search)
        {
            try
            {
                return await _patientRepository.GetTotalCountAsync(search);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total patients count");
                throw;
            }
        }
    }
}