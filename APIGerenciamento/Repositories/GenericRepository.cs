
using APIGerenciamento.Context;
using Microsoft.EntityFrameworkCore;

namespace APIGerenciamento.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected APIGerenciamentoContext _ctx;
        protected DbSet<T> _db;

        public Repository(APIGerenciamentoContext ctx)
        {
            _ctx = ctx;
            _db = ctx.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAllAsync() => await _db.ToListAsync();
        public async Task<T?> GetByIdAsync(int id) => await _db.FindAsync(id);
        public async Task AddAsync(T e) => await _db.AddAsync(e);
        public void Update(T e) => _db.Update(e);

        public void Remove(T e) => _db.Remove(e);

        public IQueryable<T> Query()
        {
            return _db.AsNoTracking();
        }
    }
}
