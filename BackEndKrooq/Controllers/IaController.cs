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
    public class IaController : ControllerBase
    {
        private readonly IaService _iaService;
        private readonly EstimativaCustoService _estimativaCustoService;
        private readonly IaImagemService _iaImagemService;

        public IaController(
            IaService iaService,
            EstimativaCustoService estimativaCustoService,
            IaImagemService iaImagemService)
        {
            _iaService = iaService;
            _estimativaCustoService = estimativaCustoService;
            _iaImagemService = iaImagemService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> EnviarMensagem(IaMensagemDTO dto)
        {
            var usuarioId = ObterUsuarioId();

            var resposta = await _iaService.EnviarMensagem(usuarioId, dto);

            if (resposta == null)
            {
                return BadRequest(new
                {
                    mensagem = "Projeto não encontrado ou mensagem inválida."
                });
            }

            return Ok(resposta);
        }

        [HttpGet("historico/{projetoId}")]
        public async Task<IActionResult> BuscarHistorico(int projetoId)
        {
            var usuarioId = ObterUsuarioId();

            var historico = await _iaService.BuscarHistorico(usuarioId, projetoId);

            if (historico == null)
            {
                return NotFound(new
                {
                    mensagem = "Projeto não encontrado."
                });
            }

            return Ok(historico);
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
        [HttpDelete("historico/{projetoId}")]
        public async Task<IActionResult> LimparHistorico(int projetoId)
        {
            var usuarioId = ObterUsuarioId();

            var apagado = await _iaService.LimparHistorico(usuarioId, projetoId);

            if (!apagado)
            {
                return NotFound(new
                {
                    mensagem = "Projeto não encontrado."
                });
            }

            return Ok(new
            {
                mensagem = "Histórico apagado com sucesso."
            });
        }
        [HttpPost("estimativa/{projetoId}")]
        public async Task<IActionResult> CalcularEstimativa(int projetoId)
        {
            var usuarioId = ObterUsuarioId();

            var estimativa = await _estimativaCustoService.CalcularEstimativa(usuarioId, projetoId);

            if (estimativa == null)
            {
                return NotFound(new
                {
                    mensagem = "Projeto não encontrado."
                });
            }

            return Ok(estimativa);
        }

        [HttpPost("imagem/gerar")]
        public async Task<IActionResult> GerarImagem(GerarImagemDTO dto)
        {
            var usuarioId = ObterUsuarioId();

            var imagem = await _iaImagemService.GerarImagem(usuarioId, dto);

            if (imagem == null)
            {
                return BadRequest(new
                {
                    mensagem = "Não foi possível gerar a imagem."
                });
            }

            if (string.IsNullOrWhiteSpace(imagem.UrlImagem))
            {
                return BadRequest(new
                {
                    mensagem = "Você atingiu o limite de 1 imagem gerada para este projeto."
                });
            }

            return Ok(imagem);
        }

        [HttpGet("imagem/projeto/{projetoId}")]
        public async Task<IActionResult> ListarImagens(int projetoId)
        {
            var usuarioId = ObterUsuarioId();

            var imagens = await _iaImagemService.ListarImagens(usuarioId, projetoId);

            if (imagens == null)
            {
                return NotFound(new
                {
                    mensagem = "Projeto não encontrado."
                });
            }

            return Ok(imagens);
        }
        [HttpGet("teste-arquivos")]
        [AllowAnonymous]
        public IActionResult TesteArquivos(
            [FromServices] IWebHostEnvironment env)
        {
            var pasta = Path.Combine(
                env.WebRootPath,
                "uploads",
                "ia"
            );

            return Ok(new
            {
                pastaExiste = Directory.Exists(pasta),
                caminho = pasta,
                arquivos = Directory.Exists(pasta)
                    ? Directory.GetFiles(pasta)
                    : []
            });
        }
        [HttpGet("teste-webroot")]
        [AllowAnonymous]
        public IActionResult TesteWebRoot(
    [FromServices] IWebHostEnvironment env)
        {
            return Ok(new
            {
                CurrentDirectory = Directory.GetCurrentDirectory(),
                WebRootPath = env.WebRootPath,
                ContentRootPath = env.ContentRootPath
            });
        }
        [HttpGet("teste-arquivo")]
        [AllowAnonymous]
        public IActionResult TesteArquivo(
    [FromServices] IWebHostEnvironment env)
        {
            var arquivo = Path.Combine(
                env.WebRootPath,
                "teste.txt"
            );

            return Ok(new
            {
                existe = System.IO.File.Exists(arquivo),
                caminho = arquivo
            });
        }
        [HttpGet("teste-static")]
        [AllowAnonymous]
        public IActionResult TesteStatic()
        {
            return PhysicalFile(
                "/app/out/wwwroot/teste.txt",
                "text/plain"
            );
        }
    }
}