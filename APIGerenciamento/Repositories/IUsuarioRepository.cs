using APIGerenciamento.Models;

namespace APIGerenciamento.Repositories
{
    public interface IUsuarioRepository  : IRepository<Usuario>
    {
        Task<Usuario> GetByEmailAsync(string email);
    }
}
