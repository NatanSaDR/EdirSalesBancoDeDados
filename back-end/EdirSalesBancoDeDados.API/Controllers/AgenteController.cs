﻿using EdirSalesBancoDeDados.Application.DTOs;
using EdirSalesBancoDeDados.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdirSalesBancoDeDados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AgenteController : ControllerBase
    {
        private readonly IAgenteUseCase _agenteUseCase;
        public AgenteController(IAgenteUseCase agenteUseCase)
        {
            _agenteUseCase = agenteUseCase;
        }

        [Authorize(Roles = "Admin, Editor, Leitor")]
        [HttpGet]
        public async Task<ActionResult<ICollection<AgenteDto>>> Listar(int pagina, int tamanhoPagina)
        {
            try
            {
                var result = await _agenteUseCase.ListarTodos(pagina, tamanhoPagina);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "Admin, Editor, Leitor")]
        [HttpGet("{id}", Name = "BuscarPorIdAgente")]
        public async Task<ActionResult<AgenteDto>> GetById(int id)
        {
            try
            {
                var result = await _agenteUseCase.BuscarPorId(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "Editor, Admin")]
        [HttpPost]
        public async Task<ActionResult<AgenteDto>> AddAgente([FromBody] AgenteDto agenteDto)
        {
            try
            {
                var result = await _agenteUseCase.AddAgente(agenteDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "Editor, Admin")]
        [HttpPut]
        public async Task<ActionResult<AgenteDto>> Atualizar(int id, [FromBody] AgenteDto agenteDto)
        {
            try
            {
                var result = await _agenteUseCase.AtualizarAgente(id, agenteDto);
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
                await _agenteUseCase.DeletarAgente(id);
                return Ok("Agente deletado");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = " Admin")]

        [HttpPost("importar")]
        public async Task<ActionResult<int>> Importar(IFormFile excelAgentes)
        {
            try
            {
                var quantidadeImportada = await _agenteUseCase.ImportarAgentes(excelAgentes);

                return Ok(new { mensagem = $"{quantidadeImportada} agentes importados com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao importar agentes.", erro = ex.Message });
            }
        }
    }
}
