using BackEndKrooq.Data;
using BackEndKrooq.DTO;
using BackEndKrooq.Models;
using BackEndKrooq.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace BackEndKrooq.Services
{
    public class ProjetoService
    {
        private readonly AppDbContext _context;

        public ProjetoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjetoRespostaDTO>> ListarPorUsuario(int usuarioId)
        {
            return await _context.Projetos
                .Where(p => p.UsuarioId == usuarioId)
                .Select(p => new ProjetoRespostaDTO
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    TipoAmbiente = p.TipoAmbiente,
                    Largura = p.Largura,
                    Comprimento = p.Comprimento,
                    Altura = p.Altura,
                    Area = p.Largura * p.Comprimento,
                    Status = p.Status,
                    DataCriacao = p.DataCriacao
                })
                .ToListAsync();
        }

        public async Task<ProjetoRespostaDTO?> BuscarPorId(int id, int usuarioId)
        {
            var projeto = await _context.Projetos
                .FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == usuarioId);

            if (projeto == null)
            {
                return null;
            }

            return new ProjetoRespostaDTO
            {
                Id = projeto.Id,
                Nome = projeto.Nome,
                Descricao = projeto.Descricao,
                TipoAmbiente = projeto.TipoAmbiente,
                Largura = projeto.Largura,
                Comprimento = projeto.Comprimento,
                Altura = projeto.Altura,
                Area = projeto.Largura * projeto.Comprimento,
                Status = projeto.Status,
                DataCriacao = projeto.DataCriacao
            };
        }

        public async Task<ProjetoRespostaDTO?> Criar(ProjetoCriarDTO dto, int usuarioId)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome) ||
                string.IsNullOrWhiteSpace(dto.Descricao) ||
                string.IsNullOrWhiteSpace(dto.TipoAmbiente))
            {
                return null;
            }

            if (dto.Largura <= 0 || dto.Comprimento <= 0 || dto.Altura <= 0)
            {
                return null;
            }

            var projeto = new Projeto
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                TipoAmbiente = dto.TipoAmbiente,
                Largura = dto.Largura,
                Comprimento = dto.Comprimento,
                Altura = dto.Altura,
                UsuarioId = usuarioId
            };

            _context.Projetos.Add(projeto);
            await _context.SaveChangesAsync();

            return new ProjetoRespostaDTO
            {
                Id = projeto.Id,
                Nome = projeto.Nome,
                Descricao = projeto.Descricao,
                TipoAmbiente = projeto.TipoAmbiente,
                Largura = projeto.Largura,
                Comprimento = projeto.Comprimento,
                Altura = projeto.Altura,
                Area = projeto.Largura * projeto.Comprimento,
                Status = projeto.Status,
                DataCriacao = projeto.DataCriacao
            };
        }

        public async Task<bool> Atualizar(int id, ProjetoAtualizarDTO dto, int usuarioId)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome) ||
                string.IsNullOrWhiteSpace(dto.Descricao) ||
                string.IsNullOrWhiteSpace(dto.TipoAmbiente) ||
                string.IsNullOrWhiteSpace(dto.Status))
            {
                return false;
            }

            if (dto.Largura <= 0 || dto.Comprimento <= 0 || dto.Altura <= 0)
            {
                return false;
            }

            var projeto = await _context.Projetos
                .FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == usuarioId);

            if (projeto == null)
            {
                return false;
            }

            projeto.Nome = dto.Nome;
            projeto.Descricao = dto.Descricao;
            projeto.TipoAmbiente = dto.TipoAmbiente;
            projeto.Largura = dto.Largura;
            projeto.Comprimento = dto.Comprimento;
            projeto.Altura = dto.Altura;
            projeto.Status = dto.Status;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Deletar(int id, int usuarioId)
        {
            var projeto = await _context.Projetos
                .FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == usuarioId);

            if (projeto == null)
            {
                return false;
            }

            _context.Projetos.Remove(projeto);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}