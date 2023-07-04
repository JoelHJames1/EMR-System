using EMRWebAPI.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        try
        {
            var result =  await _userService.Login(model);
            return result;
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error occurred during registration");
            return StatusCode(500, "An error occurred during getting users.");
        }
    }



    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto model)
    {
        try
        {
            return await _userService.Register(model);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error occurred during registration");
            return StatusCode(500, "An error occurred during getting users.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            return await _userService.GetUsers();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during getting users");
            return StatusCode(500, "An error occurred during getting users.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        try
        {
            return await _userService.GetUser(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during getting user");
            return StatusCode(500, "An error occurred during getting user.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto model)
    {
        try
        {
            return await _userService.UpdateUser(id, model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during updating user");
            return StatusCode(500, "An error occurred during updating user.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            return await _userService.DeleteUser(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during deleting user");
            return StatusCode(500, "An error occurred during deleting user.");
        }
    }
}
