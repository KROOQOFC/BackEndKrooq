namespace BackEndKrooq.DTO
{
    public class ProjetoRespostaDTO
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public string TipoAmbiente { get; set; } = string.Empty;

        public decimal Largura { get; set; }

        public decimal Comprimento { get; set; }

        public decimal Altura { get; set; }

        public decimal Area { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime DataCriacao { get; set; }
    }
}