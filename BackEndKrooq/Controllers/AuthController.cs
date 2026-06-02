using BackEndKrooq.DTO;
using BackEndKrooq.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BackEndKrooq.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("cadastro")]
        public async Task<IActionResult> Cadastrar(CadastroDTO dto)
        {
            var resultado = await _authService.Cadastrar(dto);

            if (!ResultadoFoiSucesso(resultado))
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var resultado = await _authService.Login(dto);

            if (!ResultadoFoiSucesso(resultado))
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var nome = User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var tipoUsuario = User.FindFirst("tipoUsuario")?.Value;

            return Ok(new
            {
                id,
                nome,
                email,
                tipoUsuario
            });
        }

        private bool ResultadoFoiSucesso(object resultado)
        {
            var propriedadeSucesso = resultado.GetType().GetProperty("sucesso");

            if (propriedadeSucesso == null)
            {
                return false;
            }

            var valor = propriedadeSucesso.GetValue(resultado);

            if (valor is bool sucesso)
            {
                return sucesso;
            }

            return false;
        }
    }
}