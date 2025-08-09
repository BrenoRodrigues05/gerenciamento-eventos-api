using APIGerenciamento.Models;
using APIGerenciamento.Repositories;

namespace APIGerenciamento.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Evento> Eventos { get; }
        IRepository<Participante> Participantes { get; }
        IRepository<Inscricao> Inscricoes { get; }
        IEventoRepository EventoRepository { get; }
        IParticipanteRepository ParticipanteRepository { get; }

        IUsuarioRepository Usuario { get; }
        Task<int> CommitAsync();
    }
}
