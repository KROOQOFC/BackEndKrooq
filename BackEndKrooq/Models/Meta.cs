namespace BackEndKrooq.Models
{
    public class Meta
    {
        public int Id { get; set; }

        public string Texto { get; set; } = string.Empty;

        public bool Concluida { get; set; }

        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; } = null!;
    }
}