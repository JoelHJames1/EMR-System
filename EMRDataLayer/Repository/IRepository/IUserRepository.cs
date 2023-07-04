using EMRDataLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        // Add methods specific to User here
        Task<User?> GetUserByEmailAsync(string email);
        Task<List<User>> GetAllUsersAsync();


    }
}
