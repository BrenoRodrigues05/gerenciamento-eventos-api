﻿using APIGerenciamento.Models;
using APIGerenciamento.Repositories;

namespace APIGerenciamento.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Evento> Eventos { get; }
        IRepository<Participante> Participantes { get; }
        IRepository<Inscricao> Inscricoes { get; }
        Task<int> CommitAsync();
    }
}
