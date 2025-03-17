using EdirSalesBancoDeDados.Domain;

namespace EdirSalesBancoDeDados.Application.DTOs.ViewDetailsMunicipe
{
    public class DetalheMunicipe
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public DateTime? Aniversario { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
        public string Observacao { get; set; }
        public string Email { get; set; }
        public List<string> Grupos { get; set; } = new List<string>();
        public List<TelefoneDto> Telefones { get; set; } = new List<TelefoneDto>();
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string UsuarioCadastro { get; set; }
        public string? UsuarioAlteracao { get; set; }
        public List<string> Solicitacoes { get; set; } = new List<string>();

    }
}
