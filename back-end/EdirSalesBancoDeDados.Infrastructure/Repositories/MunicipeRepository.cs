using EdirSalesBancoDeDados.Domain;
using EdirSalesBancoDeDados.Domain.Interfaces;
using EdirSalesBancoDeDados.Infrastructure.Repositories.ListarTodosPag;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http;

using System.Collections.Generic;
using System.IO;


namespace EdirSalesBancoDeDados.Infrastructure.Repositories
{
    public class MunicipeRepository : IMunicipeRepository
    {
        private readonly EdirSalesContext _context;
        public MunicipeRepository(EdirSalesContext context)
        {
            _context = context;
        }
        public async Task<Municipe> Add(Municipe municipe)
        {
            await _context.Municipes.AddAsync(municipe);
            await _context.SaveChangesAsync();
            return municipe;
        }

        public async Task Delete(Municipe municipe)
        {
            _context.Municipes.Remove(municipe);
            await _context.SaveChangesAsync();

        }
        //deletar varios municipes, usado para deletar os municipes de grupos que serao excluidos 
        public async Task DeleteRange(IEnumerable<Municipe> municipes)
        {
            _context.Municipes.RemoveRange(municipes);
            await _context.SaveChangesAsync();
        }

        public async Task<Municipe> GetById(int id)
        {
            return await _context.Municipes
                .Include(g => g.Grupos)
                .Include(t => t.Telefones)
                .Include(s => s.Solicitacoes)
                    .ThenInclude(g => g.Grupos)
                .Include(s => s.Solicitacoes)
                    .ThenInclude(m => m.Municipes)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        //esse list vai listar todos os dados incluindo os grupos que cada municipe pertence, porém, paginado
        public async Task<ICollection<Municipe>> List(int pagina, int tamanhoPagina)
        {
            return await _context.Municipes
                .Include(m => m.Grupos)
                .Include(t => t.Telefones)
                .AsQueryable()
                .Paginar(pagina, tamanhoPagina);
        }
        public async Task<Municipe> Update(Municipe municipe)
        {
            _context.Municipes.Update(municipe);
            await _context.SaveChangesAsync();
            return municipe;
        }

        public async Task<int> CountAll()
        {
            return await _context.Municipes.CountAsync();
        }



        public async Task<(int totalRegistros, List<Municipe> dados)> Filtrar(
            int? id,
            string? nome,
            string? sexo,
            DateTime? aniversario,
            DateTime? aniversarioInicio,
            DateTime? aniversarioFim,
            string? logradouro,
            string? numero,
            string? complemento,
            string? bairro,
            string? cidade,
            string? estado,
            string? cep,
            string? observacao,
            string? email,
            string? telefone,
            string? grupo,
            DateTime? dataCadastro,
            DateTime? dataCadInicio,
            DateTime? dataCadFim,
            DateTime? dataAlteracao,
            DateTime? dataAltInicio,
            DateTime? dataAltFim,
            string? usuarioCadastro,
            string? usuarioAlteracao,
            int pagina,
            int tamanhoPagina
        )
        {
            var query = _context.Municipes
                .Include(m => m.Telefones) // Inclui os telefones
                .Include(m => m.Grupos) // Inclui os grupos
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(m => m.Id == id);

            if (aniversario.HasValue)
            {
                DateTime dataSemHora = aniversario.Value.Date; // Remove a hora, deixando só a data

                query = query.Where(m => m.Aniversario.HasValue &&
                                         m.Aniversario.Value.Year == dataSemHora.Year &&
                                         m.Aniversario.Value.Month == dataSemHora.Month &&
                                         m.Aniversario.Value.Day == dataSemHora.Day);
            }

            var queryString = query.ToQueryString();
            Console.WriteLine(queryString);

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(m => m.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(sexo))
                query = query.Where(m => m.Sexo == sexo);


            if (aniversarioInicio.HasValue && aniversarioFim.HasValue)
            {
                int diaInicio = aniversarioInicio.Value.Day;
                int mesInicio = aniversarioInicio.Value.Month;
                int diaFim = aniversarioFim.Value.Day;
                int mesFim = aniversarioFim.Value.Month;

                query = query.Where(m =>
                    (m.Aniversario.HasValue && m.Aniversario.Value.Month > mesInicio) ||
                    (m.Aniversario.HasValue && m.Aniversario.Value.Month == mesInicio &&
                     m.Aniversario.Value.Day >= diaInicio) &&
                    (m.Aniversario.HasValue && m.Aniversario.Value.Month < mesFim) ||
                    (m.Aniversario.HasValue && m.Aniversario.Value.Month == mesFim &&
                     m.Aniversario.Value.Day <= diaFim)
                );
            }
            else if (aniversarioInicio.HasValue)
            {
                int diaInicio = aniversarioInicio.Value.Day;
                int mesInicio = aniversarioInicio.Value.Month;

                query = query.Where(m =>
                    (m.Aniversario.HasValue && m.Aniversario.Value.Month > mesInicio) ||
                    (m.Aniversario.HasValue && m.Aniversario.Value.Month == mesInicio &&
                     m.Aniversario.Value.Day >= diaInicio)
                );
            }
            else if (aniversarioFim.HasValue)
            {
                int diaFim = aniversarioFim.Value.Day;
                int mesFim = aniversarioFim.Value.Month;

                query = query.Where(m =>
                    (m.Aniversario.HasValue && m.Aniversario.Value.Month < mesFim) ||
                    (m.Aniversario.HasValue && m.Aniversario.Value.Month == mesFim &&
                     m.Aniversario.Value.Day <= diaFim)
                );
            }



            if (!string.IsNullOrWhiteSpace(logradouro))
                query = query.Where(m => m.Logradouro.Contains(logradouro));

            if (!string.IsNullOrWhiteSpace(numero))
                query = query.Where(m => m.Numero.Contains(numero));

            if (!string.IsNullOrWhiteSpace(complemento))
                query = query.Where(m => m.Complemento.Contains(complemento));

            if (!string.IsNullOrWhiteSpace(bairro))
                query = query.Where(m => m.Bairro.Contains(bairro));

            if (!string.IsNullOrWhiteSpace(cidade))
                query = query.Where(m => m.Cidade.Contains(cidade));

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(m => m.Estado.Contains(estado));

            if (!string.IsNullOrWhiteSpace(cep))
                query = query.Where(m => m.CEP.Contains(cep));

            if (!string.IsNullOrWhiteSpace(observacao))
                query = query.Where(m => m.Observacao.Contains(observacao));

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(m => m.Email.Contains(email));

            if (!string.IsNullOrWhiteSpace(telefone))
                query = query.Where(m => m.Telefones.Any(t => t.Numero.Contains(telefone)));


            //se houver valor em dataCadastro, busca se existe uma data de cadastro igual a dataCadastro
            if (dataCadastro.HasValue)
            {
                DateTime dataSemHora = dataCadastro.Value.Date;
                query = query.Where(m => m.DataCadastro.Date == dataSemHora);
            }

            // se houver valor em dataCadInicio e dataCadFim, busca se existe uma data de cadastro entre dataCadInicio e dataCadFim
            else
            if (dataCadInicio.HasValue && dataCadFim.HasValue)
            {
                DateTime dataSemHoraInicio = dataCadInicio.Value.Date;
                DateTime dataSemHoraFim = dataCadFim.Value.Date;
                query = query.Where(m => m.DataCadastro.Date >= dataSemHoraInicio && m.DataCadastro.Date <= dataSemHoraFim);
            }
            // se houver valor em dataCadInicio, busca se existe uma data de cadastro maior ou igual a dataCadInicio
            else if (dataCadInicio.HasValue)
            {
                DateTime dataSemHora = dataCadInicio.Value.Date;
                query = query.Where(m => m.DataCadastro.Date >= dataSemHora);
            }
            // se houver valor em dataCadFim, busca se existe uma data de cadastro menor ou igual a dataCadFim
            else if (dataCadFim.HasValue)
            {
                DateTime dataSemHora = dataCadFim.Value.Date;
                query = query.Where(m => m.DataCadastro.Date <= dataSemHora);
            }

            //se houver valor em dataAlteracao, busca se existe uma data de alteracao igual a dataAlteracao
            if (dataAlteracao.HasValue)
            {
                DateTime dataSemHora = dataAlteracao.Value.Date;
                query = query.Where(m => m.DataAlteracao.Value.Date == dataSemHora);
            }

            // se houver valor em dataAltInicio e dataAltFim, busca se existe uma data de alteracao entre dataAltInicio e dataAltFim
            else
            if (dataAltInicio.HasValue && dataAltFim.HasValue)
            {
                DateTime dataSemHoraInicio = dataAltInicio.Value.Date;
                DateTime dataSemHoraFim = dataAltFim.Value.Date;
                query = query.Where(m => m.DataAlteracao.Value.Date >= dataSemHoraInicio && m.DataAlteracao.Value.Date <= dataSemHoraFim);
            }

            // se houver valor em dataAltInicio, busca se existe uma data de alteracao maior ou igual a dataAltInicio
            else if (dataAltInicio.HasValue)
            {
                DateTime dataSemHora = dataAltInicio.Value.Date;
                query = query.Where(m => m.DataAlteracao.Value.Date >= dataSemHora);
            }

            // se houver valor em dataAltFim, busca se existe uma data de alteracao menor ou igual a dataAltFim
            else if (dataAltFim.HasValue)
            {
                DateTime dataSemHora = dataAltFim.Value.Date;
                query = query.Where(m => m.DataAlteracao.Value.Date <= dataSemHora);
            }

            if (!string.IsNullOrWhiteSpace(usuarioCadastro))
                query = query.Where(m => m.UsuarioCadastro.Contains(usuarioCadastro));

            if (!string.IsNullOrWhiteSpace(usuarioAlteracao))
                query = query.Where(m => m.UsuarioAlteracao.Contains(usuarioAlteracao));

            if (!string.IsNullOrWhiteSpace(grupo))
            {
                bool ehNumero = int.TryParse(grupo, out int idGrupoConvertido);

                query = query.Where(m => m.Grupos.Any(g =>
                    (ehNumero && g.Id == idGrupoConvertido) ||  // Se for número, busca pelo ID
                    (!ehNumero && g.NomeGrupo.Contains(grupo))  // Se for texto, busca pelo nome
                ));
            }

            // Obter o total de registros filtrados ANTES da paginação
            int totalRegistros = await query.CountAsync();

            // Aplicar paginação
            var dados = await query.AsQueryable().Paginar(pagina, tamanhoPagina);

            return (totalRegistros, dados);
        }

        //importar municipes
        public async Task<int> ImportarMunicipes(List<Municipe> listaMunicipes)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Ativa a inserção de IDs manuais
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Municipes ON");

                // Adiciona os grupos ao contexto
                await _context.Municipes.AddRangeAsync(listaMunicipes);

                // Salva as mudanças
                await _context.SaveChangesAsync();

                // Desativa o IDENTITY_INSERT
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Municipes OFF");

                // Confirma a transação
                await transaction.CommitAsync();

                return listaMunicipes.Count;
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
