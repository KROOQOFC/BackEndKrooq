using BackEndKrooq.Data;
using BackEndKrooq.DTO;
using BackEndKrooq.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEndKrooq.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public AuthService(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<object> Cadastrar(CadastroDTO dto)
        {
            var tipoUsuario = dto.TipoUsuario.ToLower().Trim();

            var tiposPermitidos = new[] { "cliente", "profissional", "fornecedor" };

            if (!tiposPermitidos.Contains(tipoUsuario))
            {
                return new
                {
                    sucesso = false,
                    mensagem = "Tipo de usuário inválido."
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Nome) ||
                string.IsNullOrWhiteSpace(dto.Sobrenome) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Telefone) ||
                string.IsNullOrWhiteSpace(dto.Senha))
            {
                return new
                {
                    sucesso = false,
                    mensagem = "Preencha todos os campos obrigatórios."
                };
            }

            var emailJaExiste = await _context.Usuarios
                .AnyAsync(u => u.Email == dto.Email);

            if (emailJaExiste)
            {
                return new
                {
                    sucesso = false,
                    mensagem = "Este e-mail já está cadastrado."
                };
            }

            if (!string.IsNullOrWhiteSpace(dto.CPF))
            {
                var cpfJaExiste = await _context.Usuarios
                    .AnyAsync(u => u.CPF == dto.CPF);

                if (cpfJaExiste)
                {
                    return new
                    {
                        sucesso = false,
                        mensagem = "Este CPF já está cadastrado."
                    };
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.CNPJ))
            {
                var cnpjJaExiste = await _context.Usuarios
                    .AnyAsync(u => u.CNPJ == dto.CNPJ);

                if (cnpjJaExiste)
                {
                    return new
                    {
                        sucesso = false,
                        mensagem = "Este CNPJ já está cadastrado."
                    };
                }
            }

            if (tipoUsuario == "cliente")
            {
                if (string.IsNullOrWhiteSpace(dto.CPF))
                {
                    return new
                    {
                        sucesso = false,
                        mensagem = "Cliente precisa informar o CPF."
                    };
                }
            }

            if (tipoUsuario == "profissional")
            {
                if (string.IsNullOrWhiteSpace(dto.CPF) ||
                    string.IsNullOrWhiteSpace(dto.AreaAtuacao) ||
                    string.IsNullOrWhiteSpace(dto.RegistroProfissional))
                {
                    return new
                    {
                        sucesso = false,
                        mensagem = "Profissional precisa informar CPF, área de atuação e registro profissional."
                    };
                }
            }

            if (tipoUsuario == "fornecedor")
            {
                if (string.IsNullOrWhiteSpace(dto.CNPJ) ||
                    string.IsNullOrWhiteSpace(dto.NomeEmpresa))
                {
                    return new
                    {
                        sucesso = false,
                        mensagem = "Fornecedor precisa informar CNPJ e nome da empresa."
                    };
                }
            }

            var usuario = new Usuario
            {
                TipoUsuario = tipoUsuario,
                Nome = dto.Nome,
                Sobrenome = dto.Sobrenome,
                Email = dto.Email,
                Telefone = dto.Telefone,
                CPF = dto.CPF,
                CNPJ = dto.CNPJ,
                AreaAtuacao = dto.AreaAtuacao,
                RegistroProfissional = dto.RegistroProfissional,
                NomeEmpresa = dto.NomeEmpresa,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha)
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return new
            {
                sucesso = true,
                mensagem = "Usuário cadastrado com sucesso."
            };
        }

        public async Task<object> Login(LoginDTO dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null)
            {
                return new
                {
                    sucesso = false,
                    mensagem = "E-mail ou senha inválidos."
                };
            }

            var senhaValida = BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash);

            if (!senhaValida)
            {
                return new
                {
                    sucesso = false,
                    mensagem = "E-mail ou senha inválidos."
                };
            }

            var token = _tokenService.GerarToken(usuario);

            var usuarioResposta = new UsuarioRespostaDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario
            };

            return new
            {
                sucesso = true,
                mensagem = "Login realizado com sucesso.",
                token,
                usuario = usuarioResposta
            };
        }
    }
}