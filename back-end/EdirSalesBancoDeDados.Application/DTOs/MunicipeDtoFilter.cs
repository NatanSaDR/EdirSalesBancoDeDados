using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdirSalesBancoDeDados.Domain;

namespace EdirSalesBancoDeDados.Application.DTOs
{
    public class MunicipeDtoFilter
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public string Aniversario { get; set; }
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
        public List<string> Grupos { get; set; } = new List<string>();
        public string DataCadastro { get; set; }
        public string? DataAlteracao { get; set; }
        public string UsuarioCadastro { get; set; }
        public string? UsuarioAlteracao { get; set; }
    }
}
