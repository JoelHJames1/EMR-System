using EMRDataLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IAddressRepository : IRepository<Address>
    {
        // Add methods specific to Address here
        Task<IEnumerable<Address>> GetAddressesByCityAsync(string city);
    }
}
