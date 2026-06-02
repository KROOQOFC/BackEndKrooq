using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEndKrooq.Models;
using Microsoft.IdentityModel.Tokens;

namespace BackEndKrooq.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GerarToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim("tipoUsuario", usuario.TipoUsuario)
            };

            var chave = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var credenciais = new SigningCredentials(
                chave,
                SecurityAlgorithms.HmacSha256
            );

            var horasExpiracao = Convert.ToDouble(_configuration["Jwt:ExpireHours"]);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(horasExpiracao),
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}