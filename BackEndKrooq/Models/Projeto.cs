namespace BackEndKrooq.Models
{
    public class Projeto
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public string TipoAmbiente { get; set; } = string.Empty;

        public decimal Largura { get; set; }

        public decimal Comprimento { get; set; }

        public decimal Altura { get; set; }

        public string Status { get; set; } = "Em andamento";

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; } = null!;

        public List<Ambiente> Ambientes { get; set; } = new();

        public List<MensagemIa> MensagensIa { get; set; } = new();
        public List<ImagemIa> ImagensIa { get; set; } = new();
    }
}