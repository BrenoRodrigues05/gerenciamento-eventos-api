using APIGerenciamento.Context;
using APIGerenciamento.Models;
using Microsoft.EntityFrameworkCore;

namespace APIGerenciamento.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
       private readonly APIGerenciamentoContext _context;

        public UsuarioRepository(APIGerenciamentoContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
   
}
