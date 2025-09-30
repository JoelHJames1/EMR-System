using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class DiagnosisService : IDiagnosisService
    {
        private readonly IDiagnosisRepository _diagnosisRepository;
        private readonly ILogger<DiagnosisService> _logger;

        public DiagnosisService(IDiagnosisRepository diagnosisRepository, ILogger<DiagnosisService> logger)
        {
            _diagnosisRepository = diagnosisRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Diagnosis>> GetPatientDiagnosesAsync(int patientId)
        {
            try
            {
                return await _diagnosisRepository.GetByPatientIdAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving diagnoses for patient {patientId}");
                throw;
            }
        }

        public async Task<IEnumerable<Diagnosis>> GetByEncounterIdAsync(int encounterId)
        {
            try
            {
                return await _diagnosisRepository.GetByEncounterIdAsync(encounterId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving diagnoses for encounter {encounterId}");
                throw;
            }
        }

        public async Task<IEnumerable<Diagnosis>> GetActiveDiagnosesAsync(int patientId)
        {
            try
            {
                return await _diagnosisRepository.GetActiveDiagnosesAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving active diagnoses for patient {patientId}");
                throw;
            }
        }

        public async Task<Diagnosis> CreateDiagnosisAsync(Diagnosis diagnosis, string userId)
        {
            try
            {
                diagnosis.CreatedDate = DateTime.UtcNow;
                diagnosis.CreatedBy = userId;
                diagnosis.DiagnosisDate = DateTime.UtcNow;
                diagnosis.ClinicalStatus = "Active";

                return await _diagnosisRepository.AddAsync(diagnosis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating diagnosis");
                throw;
            }
        }

        public async Task<Diagnosis> UpdateDiagnosisAsync(int id, Diagnosis diagnosis, string userId)
        {
            try
            {
                var existingDiagnosis = await _diagnosisRepository.GetByIdAsync(id);
                if (existingDiagnosis == null)
                {
                    throw new KeyNotFoundException($"Diagnosis with ID {id} not found");
                }

                existingDiagnosis.ClinicalStatus = diagnosis.ClinicalStatus;
                existingDiagnosis.VerificationStatus = diagnosis.VerificationStatus;
                existingDiagnosis.Severity = diagnosis.Severity;
                existingDiagnosis.Notes = diagnosis.Notes;
                existingDiagnosis.ModifiedDate = DateTime.UtcNow;
                existingDiagnosis.ModifiedBy = userId;

                return await _diagnosisRepository.UpdateAsync(existingDiagnosis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating diagnosis {id}");
                throw;
            }
        }
    }
}