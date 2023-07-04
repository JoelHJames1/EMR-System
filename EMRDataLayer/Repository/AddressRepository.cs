using EMRDataLayer.DataContext;
using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(EMRDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Address>> GetAddressesByCityAsync(string city)
        {
            return await _context.Addresses.Where(a => a.City == city).ToListAsync();
        }

    }
}
