using EdirSalesBancoDeDados.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace EdirSalesBancoDeDados.Application.Interfaces
{
    public interface IAgenteUseCase
    {
        Task<int> CountAll();

        Task<int> ImportarAgentes(IFormFile arquivo);
        Task<AgenteDto> AddAgente(AgenteDto agenteDto);
        Task DeletarAgente(int id);
        Task<AgenteDto> AtualizarAgente(int id, AgenteDto agenteDto);
        Task<AgenteDto> BuscarPorId(int id);
        Task<ICollection<AgenteDto>> ListarTodos(int pagina, int tamanhoPagina);
        //filtrar
        Task<List<AgenteDto>> Filtrar(
                int? id,
                string? agenteSolucao,
                string? contato,
                int pagina,
                int tamanhoPagina
                );
    }
}
