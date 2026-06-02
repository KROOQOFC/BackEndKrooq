using BackEndKrooq.DTO;
using BackEndKrooq.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEndKrooq.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AmbientesController : ControllerBase
    {
        private readonly AmbienteService _ambienteService;

        public AmbientesController(AmbienteService ambienteService)
        {
            _ambienteService = ambienteService;
        }

        [HttpGet("projeto/{projetoId}")]
        public async Task<IActionResult> ListarPorProjeto(int projetoId)
        {
            var usuarioId = ObterUsuarioId();

            var ambientes = await _ambienteService.ListarPorProjeto(projetoId, usuarioId);

            if (ambientes == null)
            {
                return NotFound(new
                {
                    mensagem = "Projeto não encontrado."
                });
            }

            return Ok(ambientes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var usuarioId = ObterUsuarioId();

            var ambiente = await _ambienteService.BuscarPorId(id, usuarioId);

            if (ambiente == null)
            {
                return NotFound(new
                {
                    mensagem = "Ambiente não encontrado."
                });
            }

            return Ok(ambiente);
        }

        [HttpPost("projeto/{projetoId}")]
        public async Task<IActionResult> Criar(int projetoId, AmbienteCriarDTO dto)
        {
            var usuarioId = ObterUsuarioId();

            var ambiente = await _ambienteService.Criar(projetoId, dto, usuarioId);

            if (ambiente == null)
            {
                return BadRequest(new
                {
                    mensagem = "Projeto não encontrado ou dados inválidos. Preencha todos os campos e informe medidas maiores que zero."
                });
            }

            return CreatedAtAction(nameof(BuscarPorId), new { id = ambiente.Id }, ambiente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, AmbienteAtualizarDTO dto)
        {
            var usuarioId = ObterUsuarioId();

            var atualizado = await _ambienteService.Atualizar(id, dto, usuarioId);

            if (!atualizado)
            {
                return BadRequest(new
                {
                    mensagem = "Ambiente não encontrado ou dados inválidos."
                });
            }

            return Ok(new
            {
                mensagem = "Ambiente atualizado com sucesso."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var usuarioId = ObterUsuarioId();

            var deletado = await _ambienteService.Deletar(id, usuarioId);

            if (!deletado)
            {
                return NotFound(new
                {
                    mensagem = "Ambiente não encontrado."
                });
            }

            return Ok(new
            {
                mensagem = "Ambiente deletado com sucesso."
            });
        }

        private int ObterUsuarioId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            return int.Parse(id);
        }
    }
}