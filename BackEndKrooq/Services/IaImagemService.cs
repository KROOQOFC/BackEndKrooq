using BackEndKrooq.Data;
using BackEndKrooq.DTO;
using BackEndKrooq.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace BackEndKrooq.Services
{
    public class IaImagemService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public IaImagemService(
            AppDbContext context,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ImagemIaRespostaDTO?> GerarImagem(int usuarioId, GerarImagemDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Prompt))
            {
                return null;
            }

            var projeto = await _context.Projetos
                .FirstOrDefaultAsync(p => p.Id == dto.ProjetoId && p.UsuarioId == usuarioId);

            if (projeto == null)
            {
                return null;
            }

            var quantidadeImagens = await _context.ImagensIa
                .CountAsync(i => i.ProjetoId == dto.ProjetoId);

            if (quantidadeImagens >= 1)
            {
                return new ImagemIaRespostaDTO
                {
                    ProjetoId = dto.ProjetoId,
                    Prompt = dto.Prompt,
                    UrlImagem = "",
                    DataCriacao = DateTime.UtcNow
                };
            }

            var promptCompleto = MontarPromptImagem(projeto, dto.Prompt);

            var imagemBase64 = await ChamarOpenAiImagem(promptCompleto);

            if (string.IsNullOrWhiteSpace(imagemBase64))
            {
                return null;
            }

            var urlImagem = SalvarImagemNoServidor(imagemBase64, projeto.Id);

            var imagemIa = new ImagemIa
            {
                ProjetoId = projeto.Id,
                Prompt = dto.Prompt,
                UrlImagem = urlImagem
            };

            _context.ImagensIa.Add(imagemIa);

            await _context.SaveChangesAsync();

            return new ImagemIaRespostaDTO
            {
                Id = imagemIa.Id,
                ProjetoId = imagemIa.ProjetoId,
                Prompt = imagemIa.Prompt,
                UrlImagem = imagemIa.UrlImagem,
                DataCriacao = imagemIa.DataCriacao
            };
        }

        public async Task<List<ImagemIaRespostaDTO>?> ListarImagens(int usuarioId, int projetoId)
        {
            var projetoExiste = await _context.Projetos
                .AnyAsync(p => p.Id == projetoId && p.UsuarioId == usuarioId);

            if (!projetoExiste)
            {
                return null;
            }

            return await _context.ImagensIa
                .Where(i => i.ProjetoId == projetoId)
                .OrderByDescending(i => i.DataCriacao)
                .Select(i => new ImagemIaRespostaDTO
                {
                    Id = i.Id,
                    ProjetoId = i.ProjetoId,
                    Prompt = i.Prompt,
                    UrlImagem = i.UrlImagem,
                    DataCriacao = i.DataCriacao
                })
                .ToListAsync();
        }

        private string SalvarImagemNoServidor(string imagemBase64, int projetoId)
        {
            var pastaUploads = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "ia"
            );

            if (!Directory.Exists(pastaUploads))
            {
                Directory.CreateDirectory(pastaUploads);
            }

            var nomeArquivo = $"projeto-{projetoId}-{DateTime.UtcNow.Ticks}.png";

            var caminhoCompleto = Path.Combine(pastaUploads, nomeArquivo);

            var bytesImagem = Convert.FromBase64String(imagemBase64);

            File.WriteAllBytes(caminhoCompleto, bytesImagem);

            return $"/uploads/ia/{nomeArquivo}";
        }

        private string MontarPromptImagem(Projeto projeto, string promptUsuario)
        {
            var area = projeto.Largura * projeto.Comprimento;

            return $@"
Crie uma imagem realista e profissional de design de interiores/reforma.

Contexto do projeto:
Nome: {projeto.Nome}
Descrição: {projeto.Descricao}
Tipo de ambiente: {projeto.TipoAmbiente}
Medidas aproximadas: {projeto.Largura}m x {projeto.Comprimento}m x {projeto.Altura}m
Área aproximada: {area}m²

Pedido do usuário:
{promptUsuario}

Estilo desejado:
- imagem realista;
- ambiente moderno;
- iluminação natural e elegante;
- aparência de projeto profissional de arquitetura;
- sem textos, marcas d'água ou logotipos na imagem.
";
        }

        private async Task<string?> ChamarOpenAiImagem(string prompt)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return null;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var body = new
            {
                model = "gpt-image-1",
                prompt = prompt,
                size = "1024x1024"
            };

            var json = JsonSerializer.Serialize(body);

            var response = await _httpClient.PostAsync(
                "https://api.openai.com/v1/images/generations",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("========== ERRO AO GERAR IMAGEM ==========");
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                Console.WriteLine(responseContent);
                Console.WriteLine("==========================================");

                return null;
            }

            using var document = JsonDocument.Parse(responseContent);

            var base64 = document.RootElement
                .GetProperty("data")[0]
                .GetProperty("b64_json")
                .GetString();

            return base64;
        }
    }
}