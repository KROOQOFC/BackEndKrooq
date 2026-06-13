using BackEndKrooq.DTO;
using BackEndKrooq.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEndKrooq.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TarefasController : ControllerBase
    {
        private readonly TarefaService _tarefaService;

        public TarefasController(TarefaService tarefaService)
        {
            _tarefaService = tarefaService;
        }

        [HttpGet("projeto/{projetoId}")]
        public async Task<IActionResult> ListarPorProjeto(int projetoId)
        {
            var tarefas = await _tarefaService.ListarPorProjeto(projetoId);

            return Ok(tarefas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var tarefa = await _tarefaService.BuscarPorId(id);

            if (tarefa == null)
                return NotFound();

            return Ok(tarefa);
        }

        [HttpPost]
        public async Task<IActionResult> Criar(TarefaCriarDTO dto)
        {
            var tarefa = await _tarefaService.Criar(dto);

            if (tarefa == null)
            {
                return BadRequest(new
                {
                    mensagem = "Projeto não encontrado."
                });
            }

            return Ok(tarefa);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(
            int id,
            TarefaAtualizarDTO dto)
        {
            var atualizado =
                await _tarefaService.Atualizar(id, dto);

            if (!atualizado)
                return NotFound();

            return Ok(new
            {
                mensagem = "Tarefa atualizada com sucesso."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var removido =
                await _tarefaService.Deletar(id);

            if (!removido)
                return NotFound();

            return Ok(new
            {
                mensagem = "Tarefa removida com sucesso."
            });
        }
    }
}