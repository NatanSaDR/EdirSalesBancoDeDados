using System.Text.Json.Serialization;

namespace EdirSalesBancoDeDados.Domain
{
    public class Municipe : EntityBase
    {
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
        public ICollection<Telefone> Telefones { get; set; } = new List<Telefone>();
        public ICollection<Grupo> Grupos { get; set; } = new List<Grupo>();
        public ICollection<Solicitacao> Solicitacoes { get; set; } = new List<Solicitacao>();
    }
}
