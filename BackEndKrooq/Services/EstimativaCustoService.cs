using BackEndKrooq.Data;
using BackEndKrooq.DTO;
using Microsoft.EntityFrameworkCore;

namespace BackEndKrooq.Services
{
    public class EstimativaCustoService
    {
        private readonly AppDbContext _context;
        private readonly OpenAiService _openAiService;

        public EstimativaCustoService(AppDbContext context, OpenAiService openAiService)
        {
            _context = context;
            _openAiService = openAiService;
        }

        public async Task<EstimativaCustoDTO?> CalcularEstimativa(int usuarioId, int projetoId)
        {
            var projeto = await _context.Projetos
                .FirstOrDefaultAsync(p => p.Id == projetoId && p.UsuarioId == usuarioId);

            if (projeto == null)
            {
                return null;
            }

            var area = projeto.Largura * projeto.Comprimento;

            var custoPorMetro = DefinirCustoPorMetro(projeto.TipoAmbiente);

            var valorEstimado = area * custoPorMetro;

            var observacao = "Estimativa aproximada baseada no tipo de ambiente e metragem do projeto. O valor real pode variar conforme região, materiais, mão de obra e complexidade da reforma.";

            var prompt = $@"
Você é a IA da plataforma Krooq, voltada para reformas, construção civil, arquitetura e design de interiores.

Explique de forma clara e profissional a seguinte estimativa de custo:

Projeto: {projeto.Nome}
Descrição: {projeto.Descricao}
Tipo de ambiente: {projeto.TipoAmbiente}
Área aproximada: {area}m²
Custo médio usado por m²: R$ {custoPorMetro}
Valor estimado total: R$ {valorEstimado}

A explicação deve:
- deixar claro que é uma estimativa aproximada;
- explicar que o valor pode variar por região, materiais e mão de obra;
- sugerir formas de economizar;
- ser curta e fácil de entender.
";

            var explicacaoIa = await _openAiService.GerarRespostaAsync(prompt);

            return new EstimativaCustoDTO
            {
                Area = area,
                TipoAmbiente = projeto.TipoAmbiente,
                CustoPorMetro = custoPorMetro,
                ValorEstimado = valorEstimado,
                Observacao = observacao,
                ExplicacaoIa = explicacaoIa
            };
        }

        private decimal DefinirCustoPorMetro(string tipoAmbiente)
        {
            var tipo = tipoAmbiente.ToLower().Trim();

            return tipo switch
            {
                "cozinha" => 1800,
                "banheiro" => 2200,
                "sala" => 1400,
                "quarto" => 1200,
                "lavanderia" => 1300,
                "área externa" => 1500,
                "area externa" => 1500,
                _ => 1500
            };
        }
    }
}