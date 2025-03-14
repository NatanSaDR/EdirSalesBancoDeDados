using EdirSalesBancoDeDados.Domain;
using EdirSalesBancoDeDados.Domain.Interfaces;
using EdirSalesBancoDeDados.Infrastructure.Repositories.ListarTodosPag;
using Microsoft.EntityFrameworkCore;

namespace EdirSalesBancoDeDados.Infrastructure.Repositories
{
    public class AgenteRepository : IAgenteRepository
    {
        private readonly EdirSalesContext _context;
        public AgenteRepository(EdirSalesContext context)
        {
            _context = context;
        }

        public async Task<Agente> Add(Agente agente)
        {
            await _context.Agentes.AddAsync(agente);
            await _context.SaveChangesAsync();
            return agente;
        }

        public async Task Delete(Agente agente)
        {
            _context.Agentes.Remove(agente);
            await _context.SaveChangesAsync();
        }

        public async Task<Agente> GetById(int id)
        {
            return await _context.Agentes
                .Include(c => c.Contatos)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<ICollection<Agente>> List(int pagina, int tamanhoPagina)
        {
            return await _context.Agentes
                .AsQueryable()
                .Paginar(pagina, tamanhoPagina);
        }

        public async Task<Agente> Update(Agente agente)
        {
           _context.Agentes.Update(agente);
            await _context.SaveChangesAsync();
            return agente;
        }

        public async Task<int> CountAll()
        {
            return await _context.Agentes.CountAsync();
        }

        public async Task<int> ImportarAgentes(List<Agente> listaAgentes)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Ativa a inserção de IDs manuais
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Agentes ON");

                // Adiciona os agentes ao contexto
                await _context.Agentes.AddRangeAsync(listaAgentes);

                // Salva as mudanças
                await _context.SaveChangesAsync();

                // Desativa o IDENTITY_INSERT
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Agentes OFF");

                // Confirma a transação
                await transaction.CommitAsync();

                return listaAgentes.Count;
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
