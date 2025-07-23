using APIGerenciamento.Context;
using APIGerenciamento.Models;
using APIGerenciamento.Repositories;

namespace APIGerenciamento.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly APIGerenciamentoContext _ctx;
        public IRepository<Evento> Eventos { get; }
        public IRepository<Participante> Participantes { get; }
        public IRepository<Inscricao> Inscricoes { get; }

        public UnitOfWork(APIGerenciamentoContext ctx)
        {
            _ctx = ctx;
            Eventos = new Repository<Evento>(ctx);
            Participantes = new Repository<Participante>(ctx);
            Inscricoes = new Repository<Inscricao>(ctx);
        }
        public Task<int> CommitAsync() => _ctx.SaveChangesAsync();
        public void Dispose() => _ctx.Dispose();
    }
}
