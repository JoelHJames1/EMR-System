using EMRDataLayer.DataContext;
using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository
{
    public class ProviderRepository : Repository<Provider>, IProviderRepository
    {
        public ProviderRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Provider>> GetBySpecializationAsync(string specialization)
        {
            return await _context.Providers
                .Where(p => p.Specialization == specialization && p.IsActive)
                .ToListAsync();
        }

        public async Task<Provider> GetByNPIAsync(string npiNumber)
        {
            return await _context.Providers
                .FirstOrDefaultAsync(p => p.NPI == npiNumber);
        }

        public async Task<IEnumerable<Provider>> GetActiveProvidersAsync()
        {
            return await _context.Providers
                .Where(p => p.IsActive)
                .OrderBy(p => p.LastName)
                .ToListAsync();
        }
    }
}