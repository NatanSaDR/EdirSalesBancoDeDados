using EdirSalesBancoDeDados.Domain;
using System.ComponentModel.DataAnnotations;

namespace EdirSalesBancoDeDados.Application.DTOs
{
    public class AgenteDto
    {
        public int Id { get; set; }
        [Required]
        public string AgenteSolucao { get; set; } = string.Empty;

        public List<string>? Contatos { get; set; } = new List<string>();
    }
}
