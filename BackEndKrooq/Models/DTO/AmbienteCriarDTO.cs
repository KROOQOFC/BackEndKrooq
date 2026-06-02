namespace BackEndKrooq.DTO
{
    public class AmbienteCriarDTO
    {
        public string Nome { get; set; } = string.Empty;

        public string TipoAmbiente { get; set; } = string.Empty;

        public decimal Largura { get; set; }

        public decimal Comprimento { get; set; }

        public decimal Altura { get; set; }
    }
}