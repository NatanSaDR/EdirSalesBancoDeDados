﻿using EdirSalesBancoDeDados.Application.DTOs;
using EdirSalesBancoDeDados.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdirSalesBancoDeDados.Controllers
{
    [Route("api/municipe")]
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
        [HttpGet("listartodos")]
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
        [HttpGet("{id}")]
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
        [HttpPost("cadastrar")]
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
        [HttpPut("{id}")]
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
        [HttpDelete("{id}")]
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
        [HttpGet("filtrar")]
        public async Task<ActionResult<object>> Filtrar(
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
                )
        {
            try
            {
                var resultado = await _municipeUseCase.Filtrar(
                    id, nome, sexo, aniversario, aniversarioInicio, aniversarioFim,
                    logradouro, numero, complemento, bairro, cidade, estado, cep,
                    observacao, email, telefone, grupo, dataCadastro, dataCadInicio, dataCadFim, dataAlteracao, dataAltInicio, dataAltFim,
                    usuarioCadastro, usuarioAlteracao,
                    pagina, tamanhoPagina
                );

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("importar")]
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
