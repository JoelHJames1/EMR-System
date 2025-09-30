using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IProviderRepository : IRepository<Provider>
    {
        Task<IEnumerable<Provider>> GetBySpecializationAsync(string specialization);
        Task<Provider> GetByNPIAsync(string npiNumber);
        Task<IEnumerable<Provider>> GetActiveProvidersAsync();
    }
}