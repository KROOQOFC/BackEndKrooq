using BackEndKrooq.Data;
using BackEndKrooq.DTO;
using BackEndKrooq.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEndKrooq.Services
{
    public class IaService
    {
        private readonly AppDbContext _context;
        private readonly OpenAiService _openAiService;

        public IaService(AppDbContext context, OpenAiService openAiService)
        {
            _context = context;
            _openAiService = openAiService;
        }

        public async Task<IaRespostaDTO?> EnviarMensagem(int usuarioId, IaMensagemDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Mensagem))
            {
                return null;
            }

            var projeto = await _context.Projetos
                .Include(p => p.Ambientes)
                .Include(p => p.MensagensIa)
                .FirstOrDefaultAsync(p => p.Id == dto.ProjetoId && p.UsuarioId == usuarioId);

            if (projeto == null)
            {
                return null;
            }

            var quantidadeMensagensUsuario = await _context.MensagensIa
                .CountAsync(m => m.ProjetoId == dto.ProjetoId && m.Remetente == "usuario");

            if (quantidadeMensagensUsuario >= 10)
            {
                return new IaRespostaDTO
                {
                    Resposta = "Você atingiu o limite de 10 mensagens gratuitas para este projeto."
                };
            }

            var prompt = MontarPrompt(projeto, dto.Mensagem);

            var respostaIa = await _openAiService.GerarRespostaAsync(prompt);

            var mensagemUsuario = new MensagemIa
            {
                ProjetoId = projeto.Id,
                Remetente = "usuario",
                Conteudo = dto.Mensagem
            };

            var mensagemIa = new MensagemIa
            {
                ProjetoId = projeto.Id,
                Remetente = "ia",
                Conteudo = respostaIa
            };

            _context.MensagensIa.Add(mensagemUsuario);
            _context.MensagensIa.Add(mensagemIa);

            await _context.SaveChangesAsync();

            return new IaRespostaDTO
            {
                Resposta = respostaIa
            };
        }

        public async Task<List<IaHistoricoDTO>?> BuscarHistorico(int usuarioId, int projetoId)
        {
            var projetoExiste = await _context.Projetos
                .AnyAsync(p => p.Id == projetoId && p.UsuarioId == usuarioId);

            if (!projetoExiste)
            {
                return null;
            }

            return await _context.MensagensIa
                .Where(m => m.ProjetoId == projetoId)
                .OrderBy(m => m.DataEnvio)
                .Select(m => new IaHistoricoDTO
                {
                    Remetente = m.Remetente,
                    Conteudo = m.Conteudo,
                    DataEnvio = m.DataEnvio
                })
                .ToListAsync();
        }

        public async Task<bool> LimparHistorico(int usuarioId, int projetoId)
        {
            var projetoExiste = await _context.Projetos
                .AnyAsync(p => p.Id == projetoId && p.UsuarioId == usuarioId);

            if (!projetoExiste)
            {
                return false;
            }

            var mensagens = await _context.MensagensIa
                .Where(m => m.ProjetoId == projetoId)
                .ToListAsync();

            _context.MensagensIa.RemoveRange(mensagens);

            await _context.SaveChangesAsync();

            return true;
        }

        private string MontarPrompt(Projeto projeto, string mensagemUsuario)
        {
            var areaProjeto = projeto.Largura * projeto.Comprimento;

            var ambientes = projeto.Ambientes.Any()
                ? string.Join("\n", projeto.Ambientes.Select(a =>
                    $"- {a.Nome}, tipo {a.TipoAmbiente}, medidas: {a.Largura}m x {a.Comprimento}m x {a.Altura}m, área aproximada: {a.Largura * a.Comprimento}m²"
                ))
                : "Nenhum ambiente cadastrado ainda.";

            var historico = projeto.MensagensIa.Any()
                ? string.Join("\n", projeto.MensagensIa
                    .OrderBy(m => m.DataEnvio)
                    .TakeLast(10)
                    .Select(m => $"{m.Remetente}: {m.Conteudo}"))
                : "Ainda não há histórico de conversa.";

            return $@"
Você é a inteligência artificial da plataforma Krooq.

A Krooq é uma plataforma voltada para construção civil, reformas, arquitetura, design de interiores e organização de projetos entre clientes, profissionais e fornecedores.

Sua função é ajudar o usuário a:
- entender melhor o projeto;
- ter ideias de design e reforma;
- organizar decisões;
- pensar em materiais;
- encontrar possibilidades mais econômicas;
- ter uma noção inicial de custo, sempre deixando claro que é uma estimativa.

Dados do projeto:
Nome: {projeto.Nome}
Descrição: {projeto.Descricao}
Tipo de ambiente principal: {projeto.TipoAmbiente}
Largura: {projeto.Largura}m
Comprimento: {projeto.Comprimento}m
Altura: {projeto.Altura}m
Área aproximada: {areaProjeto}m²
Status: {projeto.Status}

Ambientes cadastrados:
{ambientes}

Histórico recente da conversa:
{historico}

Mensagem atual do usuário:
{mensagemUsuario}

Responda em português do Brasil.
Use uma linguagem clara, profissional e fácil de entender.
Não invente valores exatos como se fossem orçamento final.
Quando falar de custo, diga que é uma estimativa aproximada.
";
        }
    }
}