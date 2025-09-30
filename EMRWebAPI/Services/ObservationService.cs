using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class ObservationService : IObservationService
    {
        private readonly IObservationRepository _observationRepository;
        private readonly ILogger<ObservationService> _logger;

        public ObservationService(IObservationRepository observationRepository, ILogger<ObservationService> logger)
        {
            _observationRepository = observationRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Observation>> GetPatientObservationsAsync(int patientId)
        {
            try
            {
                return await _observationRepository.GetByPatientIdAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving observations for patient {patientId}");
                throw;
            }
        }

        public async Task<IEnumerable<Observation>> GetByEncounterIdAsync(int encounterId)
        {
            try
            {
                return await _observationRepository.GetByEncounterIdAsync(encounterId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving observations for encounter {encounterId}");
                throw;
            }
        }

        public async Task<Observation?> GetLatestVitalsAsync(int patientId)
        {
            try
            {
                return await _observationRepository.GetLatestVitalsAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving latest vitals for patient {patientId}");
                throw;
            }
        }

        public async Task<Observation?> GetObservationByIdAsync(int id)
        {
            try
            {
                return await _observationRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving observation {id}");
                throw;
            }
        }

        public async Task<Observation> CreateObservationAsync(Observation observation, string userId)
        {
            try
            {
                if (observation.ObservationDateTime == default(DateTime))
                {
                    observation.ObservationDateTime = DateTime.UtcNow;
                }
                observation.CreatedDate = DateTime.UtcNow;
                observation.CreatedBy = userId;

                return await _observationRepository.AddAsync(observation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating observation");
                throw;
            }
        }

        public async Task<Observation> UpdateObservationAsync(int id, Observation observation, string userId)
        {
            try
            {
                var existingObservation = await _observationRepository.GetByIdAsync(id);
                if (existingObservation == null)
                {
                    throw new KeyNotFoundException($"Observation with ID {id} not found");
                }

                observation.Id = id;
                observation.ModifiedDate = DateTime.UtcNow;
                observation.ModifiedBy = userId;

                return await _observationRepository.UpdateAsync(observation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating observation {id}");
                throw;
            }
        }
    }
}