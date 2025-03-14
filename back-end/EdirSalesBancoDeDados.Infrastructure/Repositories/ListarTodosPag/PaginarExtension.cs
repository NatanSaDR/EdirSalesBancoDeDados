using Microsoft.EntityFrameworkCore;

namespace EdirSalesBancoDeDados.Infrastructure.Repositories.ListarTodosPag
{
    public static class PaginarExtension
    {
        public static async Task<List<T>> Paginar<T>(this IQueryable<T> query, int pagina, int tamanhoPagina)
        {
            return await query
                .Skip((pagina - 1) * tamanhoPagina)// Pula os registros de acordo com a página
                .Take(tamanhoPagina) // Limita o número de registros à quantidade da página
                .ToListAsync();
        }
    }
}
