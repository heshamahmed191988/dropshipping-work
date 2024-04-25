using Jumia.Application.Services;
using Jumia.Dtos.ViewModel;
using Jumia.Dtos.ViewModel.User;
using Jumia.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AmazonWebSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        private readonly IRoleService _roleService;

        public AccountController(IAuthService authService, IConfiguration config,IRoleService roleService)
        {
            _authService = authService;
            _config = config;
            _roleService= roleService;
        }

        [HttpPost("register")] // api/account/register
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Directly attempt to register the user
            var userCreationResult = await _authService.RegisterUserAsync(model);
            if (!userCreationResult.Succeeded)
            {
                return BadRequest(userCreationResult.Errors);
            }

            // User has been successfully created at this point
            // Now, assign the "User" role by default
            var roleName = "User"; // Default role
            var user = await _authService.GetUserByUserNameAsync(model.UserName);
            if (user != null)
            {
                // Ensure the role exists, create if not
                if (!await _roleService.RoleExistsAsync(roleName))
                {
                    await _roleService.CreateRoleAsync(new RoleViewModel { RoleName = roleName });
                }

                // Assign the user to the role
                var roleAssignmentResult = await _roleService.AddToRoleAsync(user, roleName);
                if (!roleAssignmentResult.Succeeded)
                {
                    // Handle failure (consider rolling back user creation or notify of partial success)
                    return BadRequest($"User created but failed to assign role: {roleName}");
                }
            }

            return Ok("Account successfully created with default role assigned.");
        }

        [HttpPost("login")] // api/account/login
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.SignInAsync(model);
            if (result.Succeeded)
            {
                var user = await _authService.GetUserByUserNameAsync(model.UserName);
                var token = GenerateJwtToken(user);
                return Ok(new { token = token, expiration = DateTime.Now.AddHours(1) });
            }

            if (result.IsLockedOut)
            {
                return BadRequest("Account is locked out.");
            }

            return Unauthorized("Invalid login attempt.");
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _config["JWT:ValidIssuer"],
                audience: _config["JWT:ValidAudience"], // Corrected from "ValidAudiance" to "ValidAudience"
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        
        
        [Authorize] 
        [HttpGet("currentUserId")] // api/account/currentUserId
        public IActionResult GetCurrentUserId()
        {
            // Assuming the user's ID is stored in the NameIdentifier claim
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            return Ok(new { userId = userId });
        }
       // [Authorize]
        [HttpGet("currentUserDetails")] // api/account/currentUserDetails
        public IActionResult GetCurrentUserDetails()
        {
            // Extracting the user's ID from the NameIdentifier claim
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Extracting the username from the Name claim
            var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User is not authenticated or information is missing.");
            }

            return Ok(new { UserId = userId, UserName = userName });
        }

        [HttpGet("addressid/{userId}")]
        //[Authorize] // Optional: Use this if you want to secure your endpoint
        public async Task<ActionResult<int?>> GetAddressIdByUserId(string userId)
        {
            var addressId = await _authService.GetAddressIdByUserIdAsync(userId);

            if (addressId.HasValue)
            {
                return Ok(addressId.Value);
            }
            else
            {
                return NotFound("Address or user not found.");
            }
        }
    }

}

