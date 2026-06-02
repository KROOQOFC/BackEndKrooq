using BackEndKrooq.DTO;
using BackEndKrooq.Models.DTO;
using BackEndKrooq.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEndKrooq.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjetosController : ControllerBase
    {
        private readonly ProjetoService _projetoService;

        public ProjetosController(ProjetoService projetoService)
        {
            _projetoService = projetoService;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var usuarioId = ObterUsuarioId();

            var projetos = await _projetoService.ListarPorUsuario(usuarioId);

            return Ok(projetos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var usuarioId = ObterUsuarioId();

            var projeto = await _projetoService.BuscarPorId(id, usuarioId);

            if (projeto == null)
            {
                return NotFound(new
                {
                    mensagem = "Projeto não encontrado."
                });
            }

            return Ok(projeto);
        }

        [HttpPost]
        public async Task<IActionResult> Criar(ProjetoCriarDTO dto)
        {
            var usuarioId = ObterUsuarioId();

            var projeto = await _projetoService.Criar(dto, usuarioId);

            if (projeto == null)
            {
                return BadRequest(new
                {
                    mensagem = "Dados inválidos. Preencha todos os campos e informe medidas maiores que zero."
                });
            }

            return CreatedAtAction(nameof(BuscarPorId), new { id = projeto.Id }, projeto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, ProjetoAtualizarDTO dto)
        {
            var usuarioId = ObterUsuarioId();

            var atualizado = await _projetoService.Atualizar(id, dto, usuarioId);

            if (!atualizado)
            {
                return BadRequest(new
                {
                    mensagem = "Projeto não encontrado ou dados inválidos."
                });
            }

            return Ok(new
            {
                mensagem = "Projeto atualizado com sucesso."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var usuarioId = ObterUsuarioId();

            var deletado = await _projetoService.Deletar(id, usuarioId);

            if (!deletado)
            {
                return NotFound(new
                {
                    mensagem = "Projeto não encontrado."
                });
            }

            return Ok(new
            {
                mensagem = "Projeto deletado com sucesso."
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