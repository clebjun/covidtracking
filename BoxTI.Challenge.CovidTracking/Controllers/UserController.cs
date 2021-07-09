using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BoxTI.Challenge.CovidTracking.API.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([Required] string username)
        {
            if (username == "BOXTI")
            {
                var token = await GenerateJwtToken(username);
                return Ok(new { user = username, token = token });
            }
            else
            {
                return BadRequest("Usuário inválido!");
            }
        }

        private async Task<string> GenerateJwtToken(string username)
        {
            var someSecret = "antoniocleberdossantosjunior";
            List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.Name,username),
                    new Claim(ClaimTypes.Role,"User"),

            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(someSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken SecurityToken = new JwtSecurityToken(
                issuer: "myapi.com",
                audience: "myapi.com",
                claims: claims,
                expires: DateTime.Now.AddDays(15),
                signingCredentials: credentials
                );

            return await Task.Run<string>(() => {
                return jwtTokenHandler.WriteToken(SecurityToken);
            });
        }
    }
}
