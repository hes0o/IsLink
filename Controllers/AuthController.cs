using AutoMapper;
using FreelancerPlatform.DTOs;
using FreelancerPlatform.Entities;
using FreelancerPlatform.Entities.Enums;
using FreelancerPlatform.Repositories.Interfaces;
using FreelancerPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace FreelancerPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthController(
            IUserRepository userRepository,
            ITokenService tokenService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var existing = await _userRepository.GetUserByEmailAsync(registerDto.Email);
            if (existing != null)
                return BadRequest("Email is already in use.");

            var user = _mapper.Map<User>(registerDto);

            user.PasswordHash = HashPassword(registerDto.Password);
            user.CreatedAt = DateTime.UtcNow;

            // Create a primary profile
            user.Profile = new Profile
            {
                DisplayName = registerDto.DisplayName
            };

            _userRepository.AddUser(user);

            if (!await _userRepository.SaveChangesAsync())
                return BadRequest("Failed to register user.");

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Token = _tokenService.CreateToken(user);

            return Ok(userDto);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Token = _tokenService.CreateToken(user);

            return Ok(userDto);
        }

        #region Helpers
        private string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA256,
                100_000,
                32);

            return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string stored)
        {
            var parts = stored.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);

            var testHash = KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA256,
                100_000,
                32);

            return CryptographicOperations.FixedTimeEquals(hash, testHash);
        }
        #endregion
    }
}
