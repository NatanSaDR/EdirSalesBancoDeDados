using EdirSalesBancoDeDados.Domain;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EdirSalesBancoDeDados.Application.DTOs
{
    public class MunicipeDto
    {
    
        public string Nome { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public DateTime? Aniversario { get; set; } 
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<TelefoneDto> Telefones { get; set; } = new List<TelefoneDto>();
        public List<int> Grupos { get; set; } = new List<int>();
    }
}
