using Microsoft.AspNetCore.Mvc;
using EMRWebAPI.Model;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IUserService
    {
        Task<IActionResult> GetUsers();
        Task<IActionResult> Register(UserDto model);
        Task<IActionResult> Login(LoginDto model);  
        Task<IActionResult> GetUser(int id);
        Task<IActionResult> UpdateUser(int id, UserDto model);
        Task<IActionResult> DeleteUser(int id);
    }
}
