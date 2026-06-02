using BackEndKrooq.Data;
using BackEndKrooq.DTO;
using BackEndKrooq.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEndKrooq.Services
{
    public class AmbienteService
    {
        private readonly AppDbContext _context;

        public AmbienteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AmbienteRespostaDTO>?> ListarPorProjeto(int projetoId, int usuarioId)
        {
            var projetoExiste = await _context.Projetos
                .AnyAsync(p => p.Id == projetoId && p.UsuarioId == usuarioId);

            if (!projetoExiste)
            {
                return null;
            }

            return await _context.Ambientes
                .Where(a => a.ProjetoId == projetoId)
                .Select(a => new AmbienteRespostaDTO
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    TipoAmbiente = a.TipoAmbiente,
                    Largura = a.Largura,
                    Comprimento = a.Comprimento,
                    Altura = a.Altura,
                    Area = a.Largura * a.Comprimento,
                    ProjetoId = a.ProjetoId,
                    DataCriacao = a.DataCriacao
                })
                .ToListAsync();
        }

        public async Task<AmbienteRespostaDTO?> BuscarPorId(int id, int usuarioId)
        {
            var ambiente = await _context.Ambientes
                .Include(a => a.Projeto)
                .FirstOrDefaultAsync(a => a.Id == id && a.Projeto.UsuarioId == usuarioId);

            if (ambiente == null)
            {
                return null;
            }

            return new AmbienteRespostaDTO
            {
                Id = ambiente.Id,
                Nome = ambiente.Nome,
                TipoAmbiente = ambiente.TipoAmbiente,
                Largura = ambiente.Largura,
                Comprimento = ambiente.Comprimento,
                Altura = ambiente.Altura,
                Area = ambiente.Largura * ambiente.Comprimento,
                ProjetoId = ambiente.ProjetoId,
                DataCriacao = ambiente.DataCriacao
            };
        }

        public async Task<AmbienteRespostaDTO?> Criar(int projetoId, AmbienteCriarDTO dto, int usuarioId)
        {
            var projetoExiste = await _context.Projetos
                .AnyAsync(p => p.Id == projetoId && p.UsuarioId == usuarioId);

            if (!projetoExiste)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(dto.Nome) ||
                string.IsNullOrWhiteSpace(dto.TipoAmbiente))
            {
                return null;
            }

            if (dto.Largura <= 0 || dto.Comprimento <= 0 || dto.Altura <= 0)
            {
                return null;
            }

            var ambiente = new Ambiente
            {
                Nome = dto.Nome,
                TipoAmbiente = dto.TipoAmbiente,
                Largura = dto.Largura,
                Comprimento = dto.Comprimento,
                Altura = dto.Altura,
                ProjetoId = projetoId
            };

            _context.Ambientes.Add(ambiente);
            await _context.SaveChangesAsync();

            return new AmbienteRespostaDTO
            {
                Id = ambiente.Id,
                Nome = ambiente.Nome,
                TipoAmbiente = ambiente.TipoAmbiente,
                Largura = ambiente.Largura,
                Comprimento = ambiente.Comprimento,
                Altura = ambiente.Altura,
                Area = ambiente.Largura * ambiente.Comprimento,
                ProjetoId = ambiente.ProjetoId,
                DataCriacao = ambiente.DataCriacao
            };
        }

        public async Task<bool> Atualizar(int id, AmbienteAtualizarDTO dto, int usuarioId)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome) ||
                string.IsNullOrWhiteSpace(dto.TipoAmbiente))
            {
                return false;
            }

            if (dto.Largura <= 0 || dto.Comprimento <= 0 || dto.Altura <= 0)
            {
                return false;
            }

            var ambiente = await _context.Ambientes
                .Include(a => a.Projeto)
                .FirstOrDefaultAsync(a => a.Id == id && a.Projeto.UsuarioId == usuarioId);

            if (ambiente == null)
            {
                return false;
            }

            ambiente.Nome = dto.Nome;
            ambiente.TipoAmbiente = dto.TipoAmbiente;
            ambiente.Largura = dto.Largura;
            ambiente.Comprimento = dto.Comprimento;
            ambiente.Altura = dto.Altura;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Deletar(int id, int usuarioId)
        {
            var ambiente = await _context.Ambientes
                .Include(a => a.Projeto)
                .FirstOrDefaultAsync(a => a.Id == id && a.Projeto.UsuarioId == usuarioId);

            if (ambiente == null)
            {
                return false;
            }

            _context.Ambientes.Remove(ambiente);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}