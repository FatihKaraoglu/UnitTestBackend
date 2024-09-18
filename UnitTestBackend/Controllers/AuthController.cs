using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UnitTestShared.DTO;
using UnitTestShared.Entity;

namespace UnitTestBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // POST: api/Auth/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO registrationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userExists = await _userManager.FindByNameAsync(registrationDto.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseDto { Success = false, Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = registrationDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registrationDto.Username
            };
            var result = await _userManager.CreateAsync(user, registrationDto.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseDto { Success = false, Message = "User creation failed! Please check user details and try again." });

            // Assign default role if needed
            // await _userManager.AddToRoleAsync(user, "User");

            return Ok(new AuthResponseDto { Success = true, Message = "User created successfully!" });
        }

        // POST: api/Auth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                // Add roles if using them
                // var userRoles = await _userManager.GetRolesAsync(user);
                // foreach (var role in userRoles)
                // {
                //     authClaims.Add(new Claim(ClaimTypes.Role, role));
                // }

                var jwtSettings = _configuration.GetSection("Jwt");
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    expires: DateTime.UtcNow.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new AuthResponseDto
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Success = true,
                    Message = "Login successful!"
                });
            }
            return Unauthorized(new AuthResponseDto { Success = false, Message = "Invalid credentials!" });
        }
    }
}
