using EdirSalesBancoDeDados.Application.DTOs;
using EdirSalesBancoDeDados.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdirSalesBancoDeDados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SolicitacaoController : ControllerBase
    {
        private readonly ISolicitacaoUseCase _solicitacaoUseCase;
        public SolicitacaoController(ISolicitacaoUseCase solicitacaoUseCase)
        {
            _solicitacaoUseCase = solicitacaoUseCase;
        }

        [Authorize(Roles = "Admin, Editor, Leitor")]
        [HttpGet("ListarTodos")]
        public async Task<ActionResult<ICollection<SolicitacaoDto>>> Listar(int pagina, int tamanhoPagina)
        {
            try
            {
                var result = await _solicitacaoUseCase.ListarTodos(pagina, tamanhoPagina);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize(Roles = "Admin, Editor, Leitor")]
        [HttpGet("BuscarId")]
        public async Task<ActionResult<SolicitacaoDto>> BuscarId(int id)
        {
            try
            {
                var result = await _solicitacaoUseCase.BuscarPorId(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpPost("Cadastrar")]
        public async Task<ActionResult<SolicitacaoDto>> CadastrarSolicitacao(SolicitacaoDto solicitacaoDto)
        {
            try
            {
                var user = User.Identity;
                await _solicitacaoUseCase.AddSolicitacao(solicitacaoDto);
                return Ok(solicitacaoDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpPut("Atualizar")]
        public async Task<ActionResult<SolicitacaoDto>> Atualizar(int id, [FromBody] SolicitacaoDto solicitacaoDto)
        {
            try
            {
                await _solicitacaoUseCase.AtualizarSolicitacao(id, solicitacaoDto);
                return Ok(solicitacaoDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Deletar")]
        public async Task<ActionResult> Deletar(int id)
        {
            try
            {
                await _solicitacaoUseCase.DeletarSolicitacao(id);
                return Ok("Solicitação deletada");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "Admin, Editor, Leitor")]
        [HttpGet("filtrar")]
        public async Task<IActionResult> Filtrar(
        [FromQuery] string? tipo,
        [FromQuery] string? descricao,
        [FromQuery] string? observacao,
        [FromQuery] string? sei,
        [FromQuery] string? status,
        [FromQuery] DateTime? dataFinalizado,
        [FromQuery] DateTime? dataFinalizadoInicio,
        [FromQuery] DateTime? dataFinalizadoFim,
        [FromQuery] DateTime? dataCadastroInicio,
        [FromQuery] DateTime? dataCadastroFim,
        [FromQuery] string? usuarioCadastro,
        [FromQuery] string? usuarioAlteracao,
        [FromQuery] string? grupo,
        [FromQuery] string? agente,
        [FromQuery] string? municipe,
        [FromQuery] int pagina,
        [FromQuery] int tamanhoPagina
    )
        {
            try
            {
                var resultado = await _solicitacaoUseCase.Filtrar(
                    tipo, descricao, observacao, sei, status,
                    dataFinalizado, dataFinalizadoInicio, dataFinalizadoFim,
                    dataCadastroInicio, dataCadastroFim, usuarioCadastro, usuarioAlteracao,
                    grupo, agente, municipe, pagina, tamanhoPagina);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao filtrar solicitações", erro = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("importar")]
        public async Task<ActionResult<int>> Importar(IFormFile excelSolicitacoes)
        {
            try
            {
                var quantidadeImportada = await _solicitacaoUseCase.ImportarSolicitacoes(excelSolicitacoes);

                return Ok(new { mensagem = $"{quantidadeImportada} solicitações importadas com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao importar solicitações.", erro = ex.Message });
            }
        }
    }
}
