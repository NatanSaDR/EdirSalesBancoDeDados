using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EdirSalesBancoDeDados.Application.UsuarioLogado
{
    public abstract class BaseUseCase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseUseCase(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected string GetUsuarioLogado()
        {
            var username = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("Usuário não autenticado");

            return username;
        }
    }

}
