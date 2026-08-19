using Microsoft.IdentityModel.Tokens;
using Samad.Application.Dtos;
using Samad.Application.IServices;
using Samad.Domain.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Samad.Infrastructure.External.Services
{
    public class JwtTokenGenerator(AppSettings settings) : IJwtTokenGenerator
    {
        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                 new(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new(
                ClaimTypes.Email,
                user.Email),

            new(
                ClaimTypes.Role,
                user.Role.Title),

            new(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Jwt.Key!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: settings.Jwt.Issuer,
                audience: settings.Jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(14),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}
