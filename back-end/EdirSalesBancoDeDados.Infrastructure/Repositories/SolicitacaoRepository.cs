using EdirSalesBancoDeDados.Domain;
using EdirSalesBancoDeDados.Domain.Interfaces;
using EdirSalesBancoDeDados.Infrastructure.Repositories.ListarTodosPag;
using Microsoft.EntityFrameworkCore;

namespace EdirSalesBancoDeDados.Infrastructure.Repositories
{
    public class SolicitacaoRepository : ISolicitacaoRepository
    {
        private readonly EdirSalesContext _context;
        public SolicitacaoRepository(EdirSalesContext context)
        {
            _context = context;
        }

        public async Task<Solicitacao> Add(Solicitacao solicitacao)
        {
            _context.Solicitacoes.Add(solicitacao);
            await _context.SaveChangesAsync();
            return solicitacao;
        }

        public async Task Delete(Solicitacao solicitacao)
        {
            _context.Solicitacoes.Remove(solicitacao);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRange(IEnumerable<Solicitacao> solicitacoes)
        {
            _context.Solicitacoes.RemoveRange(solicitacoes);
            await _context.SaveChangesAsync();
        }

        public async Task<Solicitacao> GetById(int id)
        {
            return await _context.Solicitacoes
                .Include(m => m.Municipes)
                .Include(g => g.Grupos)
                .Include(o => o.Oficios)
                .Include(a => a.Agentes)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<int?> GetSolicitacaoIdByOficio(string numeroOficio)
        {
            return await _context.Solicitacoes
                .Where(s => s.Oficios.Any(o => o.NumeroOficio == numeroOficio))
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Solicitacao>> List(int pagina, int tamanhoPagina)
        {
            return await _context.Solicitacoes
                .Include(m => m.Municipes)
                .Include(g => g.Grupos)
                .Include(o => o.Oficios)
                .Include(a => a.Agentes)
                .AsQueryable()
                .Paginar(pagina, tamanhoPagina);
        }

        public async Task<Solicitacao> Update(Solicitacao solicitacao)
        {
            _context.Solicitacoes.Update(solicitacao);
            await _context.SaveChangesAsync();
            return solicitacao;
        }

        public async Task<int> CountAll()
        {
            return await _context.Solicitacoes.CountAsync();
        }

        public async Task<List<Solicitacao>> Filtrar(
            int? id,
            string? tipo,
            string? descricao,
            string? observacao,
            string? sei,
            string? status,
            DateTime? dataFinalizado,
            DateTime? dataFinalizadoInicio,
            DateTime? dataFinalizadoFim,
            DateTime? dataCadastroInicio,
            DateTime? dataCadastroFim,
            string? usuarioCadastro,
            string? usuarioAlteracao,
            string? grupo,
            string? agente,
            string? municipe,
            int pagina,
            int tamanhoPagina
        )
        {
            var query = _context.Solicitacoes
                .Include(s => s.Oficios)    // Inclui os ofícios vinculados à solicitação
                .Include(s => s.Grupos)     // Inclui os grupos vinculados
                .Include(s => s.Municipes)  // Inclui os munícipes vinculados
                .Include(s => s.Agentes)    // Inclui os agentes vinculados
                .AsQueryable();

            // 🔎 Filtros opcionais para busca

            if (id.HasValue)
                query = query.Where(s => s.Id == id);

            if (!string.IsNullOrWhiteSpace(tipo))
                query = query.Where(s => s.Tipo.Contains(tipo));

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(s => s.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(observacao))
                query = query.Where(s => s.Observacao.Contains(observacao));

            if (!string.IsNullOrWhiteSpace(sei))
                query = query.Where(s => s.SEI.Contains(sei));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(s => s.Status.Contains(status));

            if (dataFinalizado.HasValue)
                query = query.Where(s => s.DataFinalizado == dataFinalizado);


            if (dataFinalizadoInicio.HasValue && dataFinalizadoFim.HasValue)
            {
                query = query.Where(s => s.DataFinalizado >= dataFinalizadoInicio && s.DataFinalizado <= dataFinalizadoFim);
            }
            else if (dataFinalizadoInicio.HasValue)
            {
                query = query.Where(s => s.DataFinalizado >= dataFinalizadoInicio);
            }
            else if (dataFinalizadoFim.HasValue)
            {
                query = query.Where(s => s.DataFinalizado <= dataFinalizadoFim);
            }

            if (dataCadastroInicio.HasValue && dataCadastroFim.HasValue)
            {
                query = query.Where(s => s.DataCadastro >= dataCadastroInicio && s.DataCadastro <= dataCadastroFim);
            }
            else if (dataCadastroInicio.HasValue)
            {
                query = query.Where(s => s.DataCadastro >= dataCadastroInicio);
            }
            else if (dataCadastroFim.HasValue)
            {
                query = query.Where(s => s.DataCadastro <= dataCadastroFim);
            }


            if (!string.IsNullOrWhiteSpace(usuarioCadastro))
                query = query.Where(s => s.UsuarioCadastro.Contains(usuarioCadastro));

            if (!string.IsNullOrWhiteSpace(usuarioAlteracao))
                query = query.Where(s => s.UsuarioAlteracao.Contains(usuarioAlteracao));

            // 🔎 Filtro por Grupo (busca por Nome ou ID)
            if (!string.IsNullOrWhiteSpace(grupo))
            {
                bool ehNumero = int.TryParse(grupo, out int idGrupoConvertido);

                query = query.Where(s => s.Grupos.Any(g =>
                    (ehNumero && g.Id == idGrupoConvertido) ||  // Se for ID, busca pelo ID
                    (!ehNumero && g.NomeGrupo.Contains(grupo))  // Se for texto, busca pelo nome
                ));
            }

            // 🔎 Filtro por Agente (busca por Nome)
            if (!string.IsNullOrWhiteSpace(agente))
            {
                bool ehNumero = int.TryParse(agente, out int idAgenteConvertido);

                query = query.Where(s => s.Agentes.Any(a =>
                    (ehNumero && a.Id == idAgenteConvertido) ||
                    (!ehNumero && a.AgenteSolucao.Contains(agente))
                ));

            }


            // 🔎 Filtro por Munícipe (busca por Nome)
            if (!string.IsNullOrWhiteSpace(municipe))
            {
                bool ehNumero = int.TryParse(municipe, out int idMunicipeConvertido);

                query = query.Where(s => s.Municipes.Any(m =>
                (!ehNumero && m.Nome.Contains(municipe) ||
                (ehNumero && m.Id == idMunicipeConvertido))
            ));
            }


            var resultados = await query.ToListAsync();

            return await query.AsQueryable().Paginar(pagina, tamanhoPagina);
        }


        public async Task<int> ImportarSolicitacoes(List<Solicitacao> listaSolicitacoes)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Ativa a inserção de IDs manuais
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Solicitacoes ON");

                // Adiciona as solicitações ao contexto
                await _context.Solicitacoes.AddRangeAsync(listaSolicitacoes);

                // Salva as mudanças
                await _context.SaveChangesAsync();

                // Desativa o IDENTITY_INSERT
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Solicitacoes OFF");

                // Confirma a transação
                await transaction.CommitAsync();

                return listaSolicitacoes.Count;
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
