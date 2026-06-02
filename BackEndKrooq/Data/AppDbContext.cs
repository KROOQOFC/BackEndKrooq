using BackEndKrooq.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEndKrooq.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Projeto> Projetos { get; set; }

        public DbSet<Ambiente> Ambientes { get; set; }

        public DbSet<MensagemIa> MensagensIa { get; set; }
        public DbSet<ImagemIa> ImagensIa { get; set; }
    }
}