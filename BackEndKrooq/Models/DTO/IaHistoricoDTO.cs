namespace BackEndKrooq.DTO
{
    public class IaHistoricoDTO
    {
        public string Remetente { get; set; } = string.Empty;

        public string Conteudo { get; set; } = string.Empty;

        public DateTime DataEnvio { get; set; }
    }
}