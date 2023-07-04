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
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(EMRDBContext context) : base(context)
        {
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var results = await _context.Users.Include(u => u.Address).ToListAsync();
            return results;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Address) // Include the Address navigation property
                .FirstOrDefaultAsync(u => u.Email == email);
        }


    }

}
