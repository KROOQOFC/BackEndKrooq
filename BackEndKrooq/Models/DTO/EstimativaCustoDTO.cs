namespace BackEndKrooq.DTO
{
    public class EstimativaCustoDTO
    {
        public decimal Area { get; set; }

        public string TipoAmbiente { get; set; } = string.Empty;

        public decimal CustoPorMetro { get; set; }

        public decimal ValorEstimado { get; set; }

        public string Observacao { get; set; } = string.Empty;

        public string ExplicacaoIa { get; set; } = string.Empty;
    }
}