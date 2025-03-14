using EdirSalesBancoDeDados.Application.DTOs;
using EdirSalesBancoDeDados.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdirSalesBancoDeDados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class MunicipeController : ControllerBase
    {
        private readonly IMunicipeUseCase _municipeUseCase;
        public MunicipeController(IMunicipeUseCase municipeUseCase)
        {
            _municipeUseCase = municipeUseCase;
        }

        [Authorize(Roles = "Admin, Editor, Leitor")]
        [HttpGet]
        public async Task<ActionResult<object>> List(int pagina, int tamanhoPagina)
        {
            try
            {
                var listaPaginada = await _municipeUseCase.ListarTodos(pagina, tamanhoPagina);
                var totalRegistros = await _municipeUseCase.CountAll(); // Chama o método correto no UseCase

                return Ok(new
                {
                    municipes = listaPaginada,
                    totalRegistros = totalRegistros
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, Editor, Leitor")]
        [HttpGet("{id}", Name = "BuscarPorId")]
        public async Task<ActionResult<MunicipeDto>> GetById(int id)
        {
            try
            {
                var result = await _municipeUseCase.BuscarPorId(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpPost]
        public async Task<ActionResult<MunicipeDto>> AddMunicipe([FromBody] MunicipeDto municipeDto)
        {
            try
            {
                var result = await _municipeUseCase.AddMunicipe(municipeDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "Admin, Editor")]
        [HttpPut]
        public async Task<ActionResult<MunicipeDto>> Atualizar(int id, [FromBody] MunicipeDto municipeDto)
        {
            try
            {
                var result = await _municipeUseCase.AtualizarMunicipe(id, municipeDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _municipeUseCase.DeletarMunicipe(id);
                return Ok("Municipe deletado");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, Editor, Leitor")]
        [HttpGet("Filtrar")]
        public async Task<ActionResult<ICollection<MunicipeDtoFilter>>> Filtrar(
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
            int pagina,
            int tamanhoPagina)
        {
            try
            {
                var result = await _municipeUseCase.Filtrar(nome, sexo, aniversario, aniversarioInicio, aniversarioFim, logradouro, numero, complemento, bairro, cidade, estado, cep, observacao, email, telefone, grupo, pagina, tamanhoPagina);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Importar")]
        public async Task<ActionResult<int>> Importar(IFormFile excelMunicipes)
        {
            try
            {
                var res = await _municipeUseCase.ImportarMunicipes(excelMunicipes);

                return Ok(res);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
