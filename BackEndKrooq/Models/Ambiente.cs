namespace BackEndKrooq.Models
{
    public class Ambiente
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string TipoAmbiente { get; set; } = string.Empty;

        public decimal Largura { get; set; }

        public decimal Comprimento { get; set; }

        public decimal Altura { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public int ProjetoId { get; set; }

        public Projeto Projeto { get; set; } = null!;
    }
}