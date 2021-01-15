using API.Interfaces;
using Core.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Configurators
{
    public class JwtConfigurator : IJwtConfigurator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtConfigurator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        private async Task<List<Claim>> GetClaims(string userName)
        {
            var user = await _userManager.FindByEmailAsync(userName);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                List<Claim> claims = new List<Claim>()
                {
                  new Claim(ClaimTypes.Name, userName)
                };

                foreach (string role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                return claims;
            }
            return null;

        }

        public string GetToken(string userName)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                issuer: "https://localhost:44342",
                audience: "https://localhost:44342",
                claims: GetClaims(userName).Result,
                expires: DateTime.Now.AddMinutes(10080),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return tokenString;
        }
    }
}
