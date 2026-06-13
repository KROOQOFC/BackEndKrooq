using BackEndKrooq.Data;
using BackEndKrooq.DTO;
using Microsoft.EntityFrameworkCore;

namespace BackEndKrooq.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDTO?> ObterDashboard(
            int usuarioId)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.Id == usuarioId);

            if (usuario == null)
                return null;

            var projetos = await _context.Projetos
                .Where(p => p.UsuarioId == usuarioId)
                .ToListAsync();

            var projetoIds = projetos
                .Select(p => p.Id)
                .ToList();

            var tarefas = await _context.Tarefas
                .Where(t => projetoIds.Contains(t.ProjetoId))
                .ToListAsync();

            var metas = await _context.Metas
                .Where(m => m.UsuarioId == usuarioId)
                .ToListAsync();

            return new DashboardDTO
            {
                NomeUsuario = usuario.Nome,

                ProjetosAtivos =
                    projetos.Count(p =>
                        p.Status != "Concluído"),

                ProjetosConcluidos =
                    projetos.Count(p =>
                        p.Status == "Concluído"),

                TarefasPendentes =
                    tarefas.Count(t =>
                        t.Status != "Concluída"),

                TarefasConcluidas =
                    tarefas.Count(t =>
                        t.Status == "Concluída"),

                MetasPendentes =
                    metas.Count(m =>
                        !m.Concluida),

                MetasConcluidas =
                    metas.Count(m =>
                        m.Concluida),

                ProgressoMedioProjetos =
                    projetos.Any()
                        ? (int)projetos.Average(p => p.Progresso)
                        : 0
            };
        }
    }
}