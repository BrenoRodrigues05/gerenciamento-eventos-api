using APIGerenciamento.Context;
using APIGerenciamento.Models;
using APIGerenciamento.Repositories;

namespace APIGerenciamento.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        public IParticipanteRepository ParticipanteRepository { get; }
        private readonly APIGerenciamentoContext _ctx;
        public IRepository<Evento> Eventos { get; }
        public IRepository<Participante> Participantes { get; }
        public IRepository<Inscricao> Inscricoes { get; }

        public IEventoRepository EventoRepository { get; }

        public IUsuarioRepository Usuario { get; }

        public UnitOfWork(APIGerenciamentoContext ctx, IParticipanteRepository participanteRepository, 
            IEventoRepository eventoRepository, IUsuarioRepository usuarioRepository)
        {
            Usuario = usuarioRepository;
            _ctx = ctx;
            Eventos = new Repository<Evento>(ctx);
            Participantes = new Repository<Participante>(ctx);
            Inscricoes = new Repository<Inscricao>(ctx);
            ParticipanteRepository = participanteRepository;
            EventoRepository = eventoRepository;
        }
        public Task<int> CommitAsync() => _ctx.SaveChangesAsync();
        public void Dispose() => _ctx.Dispose();
    }
}
