namespace BackEndKrooq.DTO
{
    public class ImagemIaRespostaDTO
    {
        public int Id { get; set; }

        public int ProjetoId { get; set; }

        public string Prompt { get; set; } = string.Empty;

        public string UrlImagem { get; set; } = string.Empty;

        public DateTime DataCriacao { get; set; }
    }
}