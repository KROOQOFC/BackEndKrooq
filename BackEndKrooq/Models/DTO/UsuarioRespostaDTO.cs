namespace BackEndKrooq.DTO
{
    public class UsuarioRespostaDTO
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Sobrenome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string TipoUsuario { get; set; } = string.Empty;
    }
}