using APIGerenciamento.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace APIGerenciamento.Context
{
    public class APIGerenciamentoContext : DbContext
    {
        public APIGerenciamentoContext(DbContextOptions<APIGerenciamentoContext> options)
            : base(options)
        { }
            public DbSet<Evento> Eventos { get; set; }
            public DbSet<Participante> Participantes { get; set; }
            public DbSet<Inscricao> Inscricoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inscricao>()
                .HasOne(i => i.Evento)
                .WithMany(e => e.Inscricoes)
                .HasForeignKey(i => i.EventoId);

            modelBuilder.Entity<Inscricao>()
                .HasOne(i => i.Participante)
                .WithMany(p => p.Inscricoes)
                .HasForeignKey(i => i.ParticipanteId);

            modelBuilder.Entity<Inscricao>()
                .HasIndex(i => new { i.EventoId, i.ParticipanteId })
                .IsUnique(); 
        }
    }
}