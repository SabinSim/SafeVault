using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeVault.Models;

namespace SafeVault.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpPost("login-token")]
        public IActionResult GenerateToken([FromQuery] string userId, [FromQuery] string role)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
            {
                return BadRequest("Invalid identity parameters.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "SuperSecretLongKeyThatIsAtLeast32BytesLong!";
            var key = Encoding.UTF8.GetBytes(jwtKey);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(15), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString, ExpiresInMinutes = 15 });
        }

        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin")] 
        public IActionResult GetAdminDashboard()
        {
            return Ok(new 
            { 
                Status = "Success",
                Message = "Welcome to the secure SafeVault administrative core dashboard." 
            });
        }
    }
}