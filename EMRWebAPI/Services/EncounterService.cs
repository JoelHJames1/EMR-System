using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class EncounterService : IEncounterService
    {
        private readonly IEncounterRepository _encounterRepository;
        private readonly ILogger<EncounterService> _logger;

        public EncounterService(IEncounterRepository encounterRepository, ILogger<EncounterService> logger)
        {
            _encounterRepository = encounterRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Encounter>> GetAllEncountersAsync()
        {
            try
            {
                return await _encounterRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all encounters");
                throw;
            }
        }

        public async Task<IEnumerable<Encounter>> GetPatientEncountersAsync(int patientId)
        {
            try
            {
                return await _encounterRepository.GetByPatientIdAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving encounters for patient {patientId}");
                throw;
            }
        }

        public async Task<IEnumerable<Encounter>> GetActiveEncountersAsync()
        {
            try
            {
                return await _encounterRepository.GetActiveEncountersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active encounters");
                throw;
            }
        }

        public async Task<IEnumerable<Encounter>> GetEncountersAsync(int? patientId, string? status)
        {
            try
            {
                var encounters = await _encounterRepository.GetAllAsync();

                if (patientId.HasValue)
                {
                    encounters = encounters.Where(e => e.PatientId == patientId.Value);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    encounters = encounters.Where(e => e.Status == status);
                }

                return encounters;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving filtered encounters");
                throw;
            }
        }

        public async Task<Encounter?> GetEncounterByIdAsync(int id)
        {
            try
            {
                return await _encounterRepository.GetByIdWithDetailsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving encounter {id}");
                throw;
            }
        }

        public async Task<Encounter> CreateEncounterAsync(Encounter encounter, string userId)
        {
            try
            {
                encounter.Status = "In Progress";
                encounter.StartDate = DateTime.UtcNow;
                encounter.CreatedDate = DateTime.UtcNow;
                encounter.CreatedBy = userId;

                return await _encounterRepository.AddAsync(encounter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating encounter");
                throw;
            }
        }

        public async Task<Encounter> UpdateEncounterAsync(int id, Encounter encounter, string userId)
        {
            try
            {
                var existingEncounter = await _encounterRepository.GetByIdAsync(id);
                if (existingEncounter == null)
                {
                    throw new KeyNotFoundException($"Encounter with ID {id} not found");
                }

                encounter.Id = id;
                encounter.ModifiedDate = DateTime.UtcNow;
                encounter.ModifiedBy = userId;

                return await _encounterRepository.UpdateAsync(encounter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating encounter {id}");
                throw;
            }
        }

        public async Task<bool> UpdateEncounterStatusAsync(int id, string status, string userId)
        {
            try
            {
                var encounter = await _encounterRepository.GetByIdAsync(id);
                if (encounter == null)
                {
                    return false;
                }

                encounter.Status = status;
                encounter.ModifiedDate = DateTime.UtcNow;
                encounter.ModifiedBy = userId;

                if (status == "Completed")
                {
                    encounter.EndDate = DateTime.UtcNow;
                }

                await _encounterRepository.UpdateAsync(encounter);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating encounter status {id}");
                throw;
            }
        }

        public async Task<IEnumerable<Encounter>> GetEncountersAsync()
        {
            return await GetAllEncountersAsync();
        }

        public async Task<IEnumerable<object>> GetEncounterDiagnosesAsync(int encounterId)
        {
            try
            {
                var encounter = await _encounterRepository.GetByIdWithDetailsAsync(encounterId);
                if (encounter == null)
                {
                    return new List<object>();
                }

                // Return diagnoses from the encounter
                if (encounter.Diagnoses == null || !encounter.Diagnoses.Any())
                {
                    return new List<object>();
                }

                var diagnoses = encounter.Diagnoses.Select(d => new {
                    Code = d.ICDCode,
                    Description = d.DiagnosisDescription
                });

                return diagnoses.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving diagnoses for encounter {encounterId}");
                throw;
            }
        }
    }
}