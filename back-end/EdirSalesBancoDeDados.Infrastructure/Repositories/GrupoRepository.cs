using EdirSalesBancoDeDados.Domain;
using EdirSalesBancoDeDados.Domain.Interfaces;
using EdirSalesBancoDeDados.Infrastructure.Repositories.ListarTodosPag;
using Microsoft.EntityFrameworkCore;

namespace EdirSalesBancoDeDados.Infrastructure.Repositories
{
    public class GrupoRepository : IGrupoRepository
    {
        private readonly EdirSalesContext _context;
        public GrupoRepository(EdirSalesContext context)
        {
            _context = context;
        }
        public async Task<Grupo> Add(Grupo grupo)
        {
            await _context.Grupos.AddAsync(grupo);
            await _context.SaveChangesAsync();
            return grupo;
        }

        public async Task Delete(Grupo grupo)
        {
            _context.Grupos.Remove(grupo);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Grupo>> List(int pagina, int tamanhoPagina)
        {
            return await _context.Grupos.AsQueryable().Paginar(pagina, tamanhoPagina);
        }

        public async Task<Grupo> Update(Grupo grupo)
        {
            _context.Grupos.Update(grupo);
            await _context.SaveChangesAsync();
            return grupo;
        }

        public async Task<Grupo> GetById(int id)
        {
            return await _context.Grupos
                .Include(m => m.Municipes)
                    .ThenInclude(m => m.Grupos)
                .Include(m => m.Municipes)
                    .ThenInclude(s => s.Solicitacoes)
                .Include(s => s.Solicitacoes)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Grupo> DetalhesDoGrupo(int id)
        {
            return await _context.Grupos
                .Include(g => g.Municipes) // Inclui os munícipes relacionados ao grupo
                .FirstOrDefaultAsync(g => g.Id == id);
        }
        public async Task<List<Grupo>> Filtrar(int? id, string? nome, int pagina, int tamanhoPagina)
        {
            var query = _context.Grupos.AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(g => g.Id == id);
            }

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(g => g.NomeGrupo.Contains(nome));
            }

            
            return await query.Paginar(pagina, tamanhoPagina);

        }

        public async Task<int> CountAll()
        {
            return await _context.Grupos.CountAsync();
        }
        public async Task<int> ImportarGrupos(List<Grupo> listGrupos)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Ativa a inserção de IDs manuais
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Grupos ON");

                // Adiciona os grupos ao contexto
                await _context.Grupos.AddRangeAsync(listGrupos);

                // Salva as mudanças
                await _context.SaveChangesAsync();

                // Desativa o IDENTITY_INSERT
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Grupos OFF");

                // Confirma a transação
                await transaction.CommitAsync();

                return listGrupos.Count;
            }
            catch (Exception)
            {
                // Em caso de erro, desfaz a transação
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
