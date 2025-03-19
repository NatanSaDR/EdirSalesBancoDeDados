using EdirSalesBancoDeDados.Application.DTOs;
using EdirSalesBancoDeDados.Application.DTOs.ViewDetailsMunicipe;
using EdirSalesBancoDeDados.Domain;
using Microsoft.AspNetCore.Http;

namespace EdirSalesBancoDeDados.Application.Interfaces
{
    public interface IMunicipeUseCase
    {
        Task<int> CountAll();
        //import
        Task<int> ImportarMunicipes(IFormFile arquivo);

        //crud
        Task<MunicipeDto> AddMunicipe(MunicipeDto municipeDto);
        Task DeletarMunicipe(int id);
        Task<MunicipeDto> AtualizarMunicipe(int id, MunicipeDto municipeDto);
        Task<DetalheMunicipe> BuscarPorId(int id);
        Task<ICollection<DetalheMunicipe>> ListarTodos(int pagina, int tamanhoPagina);
        Task<object> Filtrar(
            int? id,
            string? nome,
            string? sexo,
            string? aniversario,
            string? aniversarioInicio,
            string? aniversarioFim,
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
            string? dataCadastro,
            string? dataCadInicio,
            string? dataCadFim,
            string? dataAlteracao,
            string? dataAltInicio,
            string? dataAltFim,
            string? usuarioCadastro,
            string? usuarioAlteracao,
            int pagina,
            int tamanhoPagina
            );

    }
}
