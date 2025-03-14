using EdirSalesBancoDeDados.Application.DTOs;
using EdirSalesBancoDeDados.Application.DTOs.ViewDetails;
using EdirSalesBancoDeDados.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdirSalesBancoDeDados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class GrupoController : ControllerBase
    {
        private readonly IGrupoUseCase _grupoUseCase;
        public GrupoController(IGrupoUseCase grupoUseCase)
        {
            _grupoUseCase = grupoUseCase;
        }

        [Authorize(Roles = "Admin, Editor, Leitor")]
        [HttpGet("ListarTodos")]
        public async Task<ActionResult> Listar(int pagina, int tamanhoPagina)
        {
            try
            {
                var result = await _grupoUseCase.ListarTodos(pagina, tamanhoPagina);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, Editor, Leitor")]
        [HttpGet("{id}", Name = "DetalhesDoGrupo")]
        public async Task<ActionResult<DetalheGrupoDto>> GetById(int id)
        {
            try
            {
                var result = await _grupoUseCase.BuscarPorId(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [Authorize(Roles = "Admin, Editor")]

        [HttpPost("CadastrarGrupo")]
        public async Task<ActionResult<GrupoDto>> Add([FromBody] GrupoDto grupoDto)
        {
            var result = await _grupoUseCase.AddGrupo(grupoDto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Editor")]

        [HttpPut("{id}", Name = "Atualizar")]
        public async Task<ActionResult<GrupoDto>> Atualizar(int id, [FromBody] GrupoDto grupoDto)
        {
            try
            {
                var result = await _grupoUseCase.AtualizarGrupo(id, grupoDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Deletar(int id)
        {
            try
            {
                await _grupoUseCase.DeletarGrupo(id);
                return Ok("Grupo deletado");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("Importar")]
        public async Task<ActionResult<int>> Importar(IFormFile excelGrupos)
        {
            try
            {
                var res = await _grupoUseCase.ImportarGrupos(excelGrupos);

                return Ok(res);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, Editor, Leitor")]
        [HttpGet("Filtrar")]
        public async Task<ActionResult<GrupoDto>> Filtrar(string? nome, int pagina, int tamanhoPagina)
        {
            try
            {
                var res = await _grupoUseCase.Filtrar(nome, pagina, tamanhoPagina);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
