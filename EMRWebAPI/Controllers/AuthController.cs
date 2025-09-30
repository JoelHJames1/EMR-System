using EMRDataLayer.Model;
using EMRWebAPI.Model;
using EMRWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            JwtService jwtService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "User with this email already exists" });
                }

                var user = new User
                {
                    UserName = model.UserName ?? model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MiddleName = model.MiddleName,
                    PhoneNumber = model.PhoneNumber,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Department = model.Department,
                    JobTitle = model.JobTitle,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                // Assign roles
                if (model.Roles != null && model.Roles.Any())
                {
                    await _userManager.AddToRolesAsync(user, model.Roles);
                }

                _logger.LogInformation($"User {user.Email} registered successfully");

                return Ok(new { message = "User registered successfully", userId = user.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return StatusCode(500, new { message = "An error occurred while registering user" });
            }
        }

        /// <summary>
        /// Login with credentials
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                if (!user.IsActive)
                {
                    return Unauthorized(new { message = "User account is deactivated" });
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);

                if (!result.Succeeded)
                {
                    if (result.IsLockedOut)
                    {
                        return Unauthorized(new { message = "Account locked due to multiple failed login attempts" });
                    }
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                var token = await _jwtService.GenerateJwtToken(user);
                var refreshToken = await _jwtService.GenerateRefreshToken();

                // Update last login
                user.LastLoginDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation($"User {user.Email} logged in successfully");

                return Ok(new
                {
                    token,
                    refreshToken,
                    user = new
                    {
                        id = user.Id,
                        email = user.Email,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        roles
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        /// <summary>
        /// Refresh access token
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
        {
            try
            {
                var principal = _jwtService.GetPrincipalFromExpiredToken(model.AccessToken);
                if (principal == null)
                {
                    return BadRequest(new { message = "Invalid token" });
                }

                var email = principal.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
                if (email == null)
                {
                    return BadRequest(new { message = "Invalid token" });
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null || !user.IsActive)
                {
                    return BadRequest(new { message = "User not found or inactive" });
                }

                var newToken = await _jwtService.GenerateJwtToken(user);
                var newRefreshToken = await _jwtService.GenerateRefreshToken();

                return Ok(new
                {
                    token = newToken,
                    refreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return StatusCode(500, new { message = "An error occurred while refreshing token" });
            }
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    middleName = user.MiddleName,
                    phoneNumber = user.PhoneNumber,
                    dateOfBirth = user.DateOfBirth,
                    gender = user.Gender,
                    department = user.Department,
                    jobTitle = user.JobTitle,
                    roles,
                    lastLogin = user.LastLoginDate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, new { message = "An error occurred while retrieving profile" });
            }
        }

        /// <summary>
        /// Logout user
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }
    }
}