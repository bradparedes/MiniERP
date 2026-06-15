using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;
using MiniERP.Core.Settings;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


namespace MiniERP.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(User user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: creds
                );

                if (string.IsNullOrEmpty(_jwtSettings.SecretKey))
                {
                    throw new Exception("SecretKey está vacía");
                }

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                Console.WriteLine($"TOKEN GENERADO: {tokenString}");
                
                return tokenString;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"ERROR TOKEN: {ex.Message}");
                throw;
            }
        }
    }
}
