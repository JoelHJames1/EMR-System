using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IProviderRepository _providerRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IEncounterRepository _encounterRepository;
        private readonly ILogger<ProviderService> _logger;

        public ProviderService(
            IProviderRepository providerRepository,
            IAppointmentRepository appointmentRepository,
            IEncounterRepository encounterRepository,
            ILogger<ProviderService> logger)
        {
            _providerRepository = providerRepository;
            _appointmentRepository = appointmentRepository;
            _encounterRepository = encounterRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Provider>> GetAllProvidersAsync()
        {
            try
            {
                return await _providerRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all providers");
                throw;
            }
        }

        public async Task<IEnumerable<Provider>> GetActiveProvidersAsync()
        {
            try
            {
                return await _providerRepository.GetActiveProvidersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active providers");
                throw;
            }
        }

        public async Task<IEnumerable<Provider>> GetBySpecializationAsync(string specialization)
        {
            try
            {
                return await _providerRepository.GetBySpecializationAsync(specialization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving providers by specialization {specialization}");
                throw;
            }
        }

        public async Task<Provider?> GetProviderByIdAsync(int id)
        {
            try
            {
                return await _providerRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving provider {id}");
                throw;
            }
        }

        public async Task<Provider?> GetByNPIAsync(string npiNumber)
        {
            try
            {
                return await _providerRepository.GetByNPIAsync(npiNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving provider by NPI {npiNumber}");
                throw;
            }
        }

        public async Task<Provider> CreateProviderAsync(Provider provider, string userId)
        {
            try
            {
                provider.IsActive = true;
                provider.CreatedDate = DateTime.UtcNow;

                return await _providerRepository.AddAsync(provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating provider");
                throw;
            }
        }

        public async Task<Provider> UpdateProviderAsync(int id, Provider provider, string userId)
        {
            try
            {
                var existingProvider = await _providerRepository.GetByIdAsync(id);
                if (existingProvider == null)
                {
                    throw new KeyNotFoundException($"Provider with ID {id} not found");
                }

                provider.Id = id;
                provider.ModifiedDate = DateTime.UtcNow;

                return await _providerRepository.UpdateAsync(provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating provider {id}");
                throw;
            }
        }

        public async Task<bool> DeleteProviderAsync(int id)
        {
            try
            {
                var provider = await _providerRepository.GetByIdAsync(id);
                if (provider == null)
                {
                    return false;
                }

                provider.IsActive = false;
                await _providerRepository.UpdateAsync(provider);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting provider {id}");
                throw;
            }
        }

        public async Task<IEnumerable<Provider>> GetProvidersAsync()
        {
            return await GetAllProvidersAsync();
        }

        public async Task<IEnumerable<Provider>> GetProvidersAsync(string? specialization, bool? activeOnly)
        {
            try
            {
                var providers = await _providerRepository.GetAllAsync();

                if (!string.IsNullOrEmpty(specialization))
                {
                    providers = providers.Where(p => p.Specialization != null && p.Specialization.Contains(specialization, StringComparison.OrdinalIgnoreCase));
                }

                if (activeOnly.HasValue && activeOnly.Value)
                {
                    providers = providers.Where(p => p.IsActive);
                }

                return providers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving providers with filters");
                throw;
            }
        }

        public async Task<Provider> CreateProviderAsync(Provider provider)
        {
            return await CreateProviderAsync(provider, "system");
        }

        public async Task<Provider> UpdateProviderAsync(int id, Provider provider)
        {
            return await UpdateProviderAsync(id, provider, "system");
        }

        public async Task<IEnumerable<object>> GetProviderScheduleAsync(int id, DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today;
            var end = endDate ?? DateTime.Today.AddDays(7);
            return await GetProviderScheduleAsync(id, start, end);
        }

        public async Task<IEnumerable<object>> GetProviderScheduleAsync(int id, DateTime startDate, DateTime endDate)
        {
            try
            {
                var appointments = await _appointmentRepository.GetByProviderIdAsync(id, startDate);
                var schedule = appointments
                    .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
                    .Select(a => new
                    {
                        Id = a.Id,
                        Date = a.AppointmentDate,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        PatientId = a.PatientId,
                        Status = a.Status,
                        Type = a.AppointmentType
                    });

                return schedule.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving schedule for provider {id}");
                throw;
            }
        }

        public async Task<object> GetProviderStatisticsAsync(int id)
        {
            try
            {
                var provider = await _providerRepository.GetByIdAsync(id);
                if (provider == null)
                {
                    throw new KeyNotFoundException($"Provider with ID {id} not found");
                }

                // Get statistics
                var allAppointments = await _appointmentRepository.GetAllAsync();
                var providerAppointments = allAppointments.Where(a => a.ProviderId == id);

                var allEncounters = await _encounterRepository.GetAllAsync();
                var providerEncounters = allEncounters.Where(e => e.ProviderId == id);

                var stats = new
                {
                    ProviderId = id,
                    TotalAppointments = providerAppointments.Count(),
                    CompletedAppointments = providerAppointments.Count(a => a.Status == "Completed"),
                    TotalEncounters = providerEncounters.Count(),
                    ActiveEncounters = providerEncounters.Count(e => e.Status == "In Progress")
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving statistics for provider {id}");
                throw;
            }
        }
    }
}