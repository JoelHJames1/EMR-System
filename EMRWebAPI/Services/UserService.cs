using AutoMapper;
using EMRWebAPI.Model;
using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TokenOptions = EMRWebAPI.Model.TokenOptions;
using EMRWebAPI.Services.IServices;

namespace EMRWebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly TokenOptions _tokenOptions;
        private readonly IRepository<User> _userRepository;
        private readonly IUserRepository _MainUserRepository;

        public UserService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration config,
            IMapper mapper,
            IRepository<User> userRepository,
            IUserRepository mainUserRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _userRepository = userRepository;
            _MainUserRepository = mainUserRepository;

            _tokenOptions = new TokenOptions();
            config.GetSection("TokenOptions").Bind(_tokenOptions);
        }

        public async Task<IActionResult> Register(UserDto model)
        {
            var user = _mapper.Map<User>(model);

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userRepository.AddAsync(user);
                return new OkResult();
            }

            var errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add(error.Description);
            }

            return new BadRequestObjectResult(errors);
        }

        public async Task<IActionResult> Login(LoginDto model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UnauthorizedResult(); // User not found
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (signInResult.Succeeded)
            {
                // Include the necessary user information in the response
                var userDto = new
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                    // Include other relevant properties
                };

                var token = GenerateToken(user);
                // Return the token and the user object
                return new OkObjectResult(new { Token = token, User = userDto });
            }

            return new UnauthorizedResult(); // Invalid credentials
        }


        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.Secret));

            // Check if the key size is less than 256 bits and adjust it if needed
            if (securityKey.KeySize < 256)
            {
                var existingKeyBytes = securityKey.Key;
                var newKeyBytes = new byte[256 / 8];
                Array.Copy(existingKeyBytes, newKeyBytes, existingKeyBytes.Length);
                securityKey = new SymmetricSecurityKey(newKeyBytes);
            }

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        // Add any additional claims here
    };

            var token = new JwtSecurityToken(
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_tokenOptions.ExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<IActionResult> GetUsers()
        {
            var users = await _MainUserRepository.GetAllUsersAsync();
            var userDtos = _mapper.Map<List<UserDto>>(users);
            return new OkObjectResult(userDtos);
        }


        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return new NotFoundResult();
            }

            var userDto = _mapper.Map<UserDto>(user);
            return new OkObjectResult(userDto);
        }

        public async Task<IActionResult> UpdateUser(int id, UserDto model)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return new NotFoundResult();
            }

            _mapper.Map(model, user);
            _userRepository.Update(user);

            return new OkResult();
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return new NotFoundResult();
            }

            _userRepository.Delete(user);

            return new OkResult();
        }
    }
}
