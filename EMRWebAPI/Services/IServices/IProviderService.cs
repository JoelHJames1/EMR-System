using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IProviderService
    {
        Task<IEnumerable<Provider>> GetAllProvidersAsync();
        Task<IEnumerable<Provider>> GetActiveProvidersAsync();
        Task<IEnumerable<Provider>> GetBySpecializationAsync(string specialization);
        Task<Provider?> GetProviderByIdAsync(int id);
        Task<Provider?> GetByNPIAsync(string npiNumber);
        Task<Provider> CreateProviderAsync(Provider provider, string userId);
        Task<Provider> UpdateProviderAsync(int id, Provider provider, string userId);
        Task<bool> DeleteProviderAsync(int id);
    }
}