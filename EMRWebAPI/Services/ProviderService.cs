using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IProviderRepository _providerRepository;
        private readonly ILogger<ProviderService> _logger;

        public ProviderService(IProviderRepository providerRepository, ILogger<ProviderService> logger)
        {
            _providerRepository = providerRepository;
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
                provider.CreatedBy = userId;

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
                provider.ModifiedBy = userId;

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
    }
}