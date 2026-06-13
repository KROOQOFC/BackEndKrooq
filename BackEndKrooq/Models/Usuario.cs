namespace BackEndKrooq.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Sobrenome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        public string? CPF { get; set; }

        public string? CNPJ { get; set; }

        public string TipoUsuario { get; set; } = string.Empty;

        public string? AreaAtuacao { get; set; }

        public string? RegistroProfissional { get; set; }

        public string? NomeEmpresa { get; set; }

        public string SenhaHash { get; set; } = string.Empty;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public bool Ativo { get; set; } = true;
        public List<Projeto> Projetos { get; set; } = new();
        public List<Meta> Metas { get; set; } = new();
    }
}