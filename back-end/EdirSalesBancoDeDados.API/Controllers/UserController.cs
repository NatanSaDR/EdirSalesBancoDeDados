using EdirSalesBancoDeDados.Application.DTOs.UserRequest;
using EdirSalesBancoDeDados.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserUseCase _userUseCase;
    private readonly TokenService _tokenService;

    public UserController(IUserUseCase userUseCase, TokenService tokenService)
    {
        _userUseCase = userUseCase;
        _tokenService = tokenService;
    }
    [HttpGet("validate-token")]
    public ActionResult<bool> ValidateToken()
    {
        if (User.Identity.IsAuthenticated)
        {
            return true;
        }
        return false;
    }



    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userUseCase.AuthenticateAsync(request.Username, request.Password);
        if (user == null)
        {
            return Unauthorized(new { message = "Usuário ou senha inválidos" });
        }

        var token = _tokenService.GenerateToken(user);
        return Ok(new { token }); // Retorna um JSON com a chave correta
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("listartodos")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var lista = await _userUseCase.List();
            var totalRegistros = await _userUseCase.CountAll(); // Chama o método correto no UseCase

            return Ok(new
            {
                solicitacoes = lista,
                totalRegistros = totalRegistros
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userUseCase.GetById(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("cadastrar")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        await _userUseCase.Add(request.Username, request.Password, request.Role);
        return Created("", new { Message = "Usuário criado com sucesso" });
    }


    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUserRequest request)
    {
        await _userUseCase.Update(id, request.Username, request.Password, request.Role);
        return Ok(new { Message = "Usuário atualizado com sucesso" });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _userUseCase.Delete(id);
        return Ok(new { Message = "Usuário deletado com sucesso" });
    }
}




