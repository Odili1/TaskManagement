using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Domain.Dtos;
using TaskManagement.Domain.UserManagement;
using TaskManagement.Persistence;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TaskManagementDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(TaskManagementDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // IMPLEMENT REGISTER ENDPOINT
        // Validate the inputs
        // check if the email already exist in the db.
        // Create the user object
        // hash the password - Bcrypt
        // Save user to db
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegistrationDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isUserExist = await _context.Users.AnyAsync(em => em.Email == userDto.Email);

            if (isUserExist)
            {
                return BadRequest("Email is already in use");
            }

            var user = new User
            {
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                AccountType = userDto.AccountType,
                UserStatus = UserStatus.Active,
                Gender = userDto.Gender,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash),
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User Registered successfully", UserId = user.Id });
        }


        // IMPLEMENT LOGIN ENPOINT
        // check if the email and password are provided (validation)
        //find user by email in the db
        // check if the account is suspended (option)
        // generate token
        // return a message (token)
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.PasswordHash))
            {
                return BadRequest("Email and password are required");
            }

            var user = await _context.Users.FirstOrDefaultAsync(em => em.Email == loginRequest.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            if (!VerifyPassword(loginRequest.PasswordHash, user.PasswordHash!))
            {
                return Unauthorized("Invalid email or password");
            }

            string token = GenerateJwtToken(user);

            return Ok(new LoginResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Token = token
            });
        }

        private string GenerateJwtToken(User user)
        {
            // CLAIM
            // 1. Attach the user ID to the name identifier in the claim
            // 2. Attach the user email to the claim email
            // 3. User FirstName to the claim name
            // 4. User FirstName and Lastname to the given name in the claim
            // 5. User account type to the role in the claim

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, $"{user.Id}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.AccountType.ToString())
            };


            // Key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key"]));



            // Credential
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            // Assign the claims, the key and creds to the token
            var token = new JwtSecurityToken(
                issuer: _configuration["Issuer"],
                audience: _configuration["Audience"],
                claims: claims,
                signingCredentials: creds,
                expires: DateTime.Now.AddHours(1)
                );


            // Final is the JwSecurity token handler that creates the token of strings
            var newToken = new JwtSecurityTokenHandler().WriteToken(token);

            return newToken;
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            {
                return false;
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, storedHash);

            return isPasswordValid;
        }

        // Go to User controller and protect the endpoints
    }
}
