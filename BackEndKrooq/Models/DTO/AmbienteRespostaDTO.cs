namespace BackEndKrooq.DTO
{
    public class AmbienteRespostaDTO
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string TipoAmbiente { get; set; } = string.Empty;

        public decimal Largura { get; set; }

        public decimal Comprimento { get; set; }

        public decimal Altura { get; set; }

        public decimal Area { get; set; }

        public int ProjetoId { get; set; }

        public DateTime DataCriacao { get; set; }
    }
}