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
        private readonly IAllergyRepository _allergyRepository;
        private readonly IObservationRepository _observationRepository;
        private readonly ILogger<PatientService> _logger;

        public PatientService(
            IPatientRepository patientRepository,
            IAllergyRepository allergyRepository,
            IObservationRepository observationRepository,
            ILogger<PatientService> logger)
        {
            _patientRepository = patientRepository;
            _allergyRepository = allergyRepository;
            _observationRepository = observationRepository;
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

        public async Task<(IEnumerable<Patient>, int)> GetPatientsAsync(int page, int pageSize, string? search)
        {
            try
            {
                var patients = await _patientRepository.GetAllWithPaginationAsync(page, pageSize, search);
                var totalCount = await _patientRepository.GetTotalCountAsync(search);
                return (patients, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patients with count");
                throw;
            }
        }

        public async Task<Patient> CreatePatientAsync(Patient patient, string userId)
        {
            try
            {
                patient.CreatedDate = DateTime.UtcNow;
                patient.CreatedBy = userId;
                patient.IsActive = true;
                return await _patientRepository.AddAsync(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient");
                throw;
            }
        }

        public async Task<Patient> UpdatePatientAsync(int id, Patient patient, string userId)
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
                patient.ModifiedBy = userId;
                return await _patientRepository.UpdateAsync(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating patient {id}");
                throw;
            }
        }

        public async Task<bool> DeletePatientAsync(int id, string userId)
        {
            try
            {
                var patient = await _patientRepository.GetByIdAsync(id);
                if (patient == null)
                {
                    return false;
                }

                patient.IsActive = false;
                patient.ModifiedDate = DateTime.UtcNow;
                patient.ModifiedBy = userId;
                await _patientRepository.UpdateAsync(patient);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting patient {id}");
                throw;
            }
        }

        public async Task<IEnumerable<Allergy>> GetPatientAllergiesAsync(int id)
        {
            try
            {
                return await _allergyRepository.GetByPatientIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving allergies for patient {id}");
                throw;
            }
        }

        public async Task<IEnumerable<Observation>> GetPatientVitalsAsync(int id)
        {
            try
            {
                return await _observationRepository.GetByPatientIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving vitals for patient {id}");
                throw;
            }
        }
    }
}