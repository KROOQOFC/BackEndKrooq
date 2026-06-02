namespace BackEndKrooq.Models
{
    public class ImagemIa
    {
        public int Id { get; set; }

        public int ProjetoId { get; set; }

        public Projeto Projeto { get; set; } = null!;

        public string Prompt { get; set; } = string.Empty;

        public string UrlImagem { get; set; } = string.Empty;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}