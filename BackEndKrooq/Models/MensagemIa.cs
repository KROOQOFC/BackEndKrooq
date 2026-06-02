namespace BackEndKrooq.Models
{
    public class MensagemIa
    {
        public int Id { get; set; }

        public int ProjetoId { get; set; }

        public Projeto Projeto { get; set; } = null!;

        public string Remetente { get; set; } = string.Empty;
        // usuario ou ia

        public string Conteudo { get; set; } = string.Empty;

        public DateTime DataEnvio { get; set; } = DateTime.UtcNow;
    }
}