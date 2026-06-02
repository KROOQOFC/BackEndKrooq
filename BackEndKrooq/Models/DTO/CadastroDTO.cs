namespace BackEndKrooq.DTO
{
    public class CadastroDTO
    {
        public string TipoUsuario { get; set; } = string.Empty;

        public string Nome { get; set; } = string.Empty;

        public string Sobrenome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        public string? CPF { get; set; }

        public string? CNPJ { get; set; }

        public string? AreaAtuacao { get; set; }

        public string? RegistroProfissional { get; set; }

        public string? NomeEmpresa { get; set; }

        public string Senha { get; set; } = string.Empty;
    }
}