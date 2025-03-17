using EdirSalesBancoDeDados.Application.DTOs;
using EdirSalesBancoDeDados.Application.DTOs.ViewDetails;
using EdirSalesBancoDeDados.Domain;
using Microsoft.AspNetCore.Http;

namespace EdirSalesBancoDeDados.Application.Interfaces
{
    public interface IGrupoUseCase
    {
        Task<int> CountAll();

        //import
        Task<int> ImportarGrupos(IFormFile arquivo);

        //crud
        Task<GrupoDto> AddGrupo(GrupoDto grupoDto);
        Task DeletarGrupo(int id);
        Task<GrupoDto> AtualizarGrupo(int id, GrupoDto grupoDto);
        Task<DetalheGrupoDto> BuscarPorId(int id);
        Task<IEnumerable<GrupoDto>> ListarTodos(int pagina, int tamanhoPagina);
        Task<List<GrupoDto>> Filtrar(int? id, string? nome, int pagina, int tamanhoPagina);
    }
}
