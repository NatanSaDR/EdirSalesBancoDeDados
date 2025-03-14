using EdirSalesBancoDeDados.Application.DTOs;
using EdirSalesBancoDeDados.Domain;
using Microsoft.AspNetCore.Http;

namespace EdirSalesBancoDeDados.Application.Interfaces
{
    public interface ISolicitacaoUseCase
    {
        Task<int> CountAll();

        Task<int> ImportarSolicitacoes(IFormFile arquivo);
        Task<List<SolicitacaoDto>> Filtrar(
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
                );
        Task<SolicitacaoDto> AddSolicitacao(SolicitacaoDto solicitacaoDto);
        Task DeletarSolicitacao(int id);
        Task<SolicitacaoDto> AtualizarSolicitacao(int id, SolicitacaoDto solicitacaoDto);
        Task<SolicitacaoDto> BuscarPorId(int id);
        Task<ICollection<SolicitacaoDto>> ListarTodos(int pagina, int tamanhoPagina);
    }
}
