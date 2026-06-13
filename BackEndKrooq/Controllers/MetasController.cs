using BackEndKrooq.Models;
using BackEndKrooq.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEndKrooq.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetaController : ControllerBase
    {
        private readonly MetaService _service;

        public MetaController(MetaService service)
        {
            _service = service;
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> Listar(int usuarioId)
        {
            var metas = await _service.ListarPorUsuario(usuarioId);

            return Ok(metas);
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Meta meta)
        {
            var nova = await _service.Criar(meta);

            return Ok(nova);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, Meta meta)
        {
            var sucesso = await _service.Atualizar(id, meta);

            if (!sucesso)
                return NotFound();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var sucesso = await _service.Excluir(id);

            if (!sucesso)
                return NotFound();

            return Ok();
        }
    }
}