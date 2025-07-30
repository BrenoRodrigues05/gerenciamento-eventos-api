using APIGerenciamento.Repositories;
using Microsoft.EntityFrameworkCore;

namespace APIGerenciamento.Pagination
{
    public class PaginationHelp
    {
        public static async Task<PagedResult<TDto>> CreateAsync<TEntity, TDto>(
           IQueryable<TEntity> query,
           int pageNumber,
           int pageSize,
           Func<TEntity, TDto> converter)
           where TEntity : class
        {
            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtoList = items.Select(converter).ToList();

            return new PagedResult<TDto>
            {
                Items = dtoList,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
