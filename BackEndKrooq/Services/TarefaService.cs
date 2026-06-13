using BackEndKrooq.Data;
using BackEndKrooq.DTO;
using BackEndKrooq.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEndKrooq.Services
{
    public class TarefaService
    {
        private readonly AppDbContext _context;

        public TarefaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TarefaRespostaDTO>> ListarPorProjeto(int projetoId)
        {
            return await _context.Tarefas
                .Where(t => t.ProjetoId == projetoId)
                .Select(t => new TarefaRespostaDTO
                {
                    Id = t.Id,
                    Titulo = t.Titulo,
                    Descricao = t.Descricao,
                    Progresso = t.Progresso,
                    Status = t.Status,
                    DataInicio = t.DataInicio,
                    DataFim = t.DataFim,
                    ProjetoId = t.ProjetoId
                })
                .ToListAsync();
        }

        public async Task<TarefaRespostaDTO?> BuscarPorId(int id)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);

            if (tarefa == null)
                return null;

            return new TarefaRespostaDTO
            {
                Id = tarefa.Id,
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Progresso = tarefa.Progresso,
                Status = tarefa.Status,
                DataInicio = tarefa.DataInicio,
                DataFim = tarefa.DataFim,
                ProjetoId = tarefa.ProjetoId
            };
        }

        public async Task<TarefaRespostaDTO?> Criar(TarefaCriarDTO dto)
        {
            var projetoExiste = await _context.Projetos
                .AnyAsync(p => p.Id == dto.ProjetoId);

            if (!projetoExiste)
                return null;

            var tarefa = new Tarefa
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Progresso = dto.Progresso,
                Status = dto.Status,
                DataInicio = dto.DataInicio,
                DataFim = dto.DataFim,
                ProjetoId = dto.ProjetoId
            };

            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();

            return new TarefaRespostaDTO
            {
                Id = tarefa.Id,
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Progresso = tarefa.Progresso,
                Status = tarefa.Status,
                DataInicio = tarefa.DataInicio,
                DataFim = tarefa.DataFim,
                ProjetoId = tarefa.ProjetoId
            };
        }

        public async Task<bool> Atualizar(int id, TarefaAtualizarDTO dto)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);

            if (tarefa == null)
                return false;

            tarefa.Titulo = dto.Titulo;
            tarefa.Descricao = dto.Descricao;
            tarefa.Progresso = dto.Progresso;
            tarefa.Status = dto.Status;
            tarefa.DataInicio = dto.DataInicio;
            tarefa.DataFim = dto.DataFim;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Deletar(int id)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);

            if (tarefa == null)
                return false;

            _context.Tarefas.Remove(tarefa);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}