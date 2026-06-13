using BackEndKrooq.Data;
using BackEndKrooq.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEndKrooq.Services
{
    public class MetaService
    {
        private readonly AppDbContext _context;

        public MetaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Meta>> ListarPorUsuario(int usuarioId)
        {
            return await _context.Metas
                .Where(m => m.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<Meta> Criar(Meta meta)
        {
            _context.Metas.Add(meta);
            await _context.SaveChangesAsync();

            return meta;
        }

        public async Task<bool> Atualizar(int id, Meta metaAtualizada)
        {
            var meta = await _context.Metas.FindAsync(id);

            if (meta == null)
                return false;

            meta.Texto = metaAtualizada.Texto;
            meta.Concluida = metaAtualizada.Concluida;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Excluir(int id)
        {
            var meta = await _context.Metas.FindAsync(id);

            if (meta == null)
                return false;

            _context.Metas.Remove(meta);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}