using EMRDataLayer.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IProviderService
    {
        Task<IEnumerable<Provider>> GetAllProvidersAsync();
        Task<IEnumerable<Provider>> GetProvidersAsync();
        Task<IEnumerable<Provider>> GetProvidersAsync(string? specialization, bool? activeOnly);
        Task<IEnumerable<Provider>> GetActiveProvidersAsync();
        Task<IEnumerable<Provider>> GetBySpecializationAsync(string specialization);
        Task<Provider?> GetProviderByIdAsync(int id);
        Task<Provider?> GetByNPIAsync(string npiNumber);
        Task<IEnumerable<object>> GetProviderScheduleAsync(int id, DateTime startDate, DateTime endDate);
        Task<IEnumerable<object>> GetProviderScheduleAsync(int id, DateTime? startDate, DateTime? endDate);
        Task<object> GetProviderStatisticsAsync(int id);
        Task<Provider> CreateProviderAsync(Provider provider);
        Task<Provider> CreateProviderAsync(Provider provider, string userId);
        Task<Provider> UpdateProviderAsync(int id, Provider provider);
        Task<Provider> UpdateProviderAsync(int id, Provider provider, string userId);
        Task<bool> DeleteProviderAsync(int id);
    }
}