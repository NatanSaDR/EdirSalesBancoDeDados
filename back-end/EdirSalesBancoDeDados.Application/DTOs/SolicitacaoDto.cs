using EdirSalesBancoDeDados.Domain;

namespace EdirSalesBancoDeDados.Application.DTOs
{
    public class SolicitacaoDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public string SEI { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? DataFinalizado { get; set; }
        public List<string>? Oficios { get; set; } = new List<string>();
        public List<string>? IdGrupos { get; set; } = new List<string>(); 
        public List<string>? IdMunicipes { get; set; } = new List<string>(); 
        public List<string>? IdAgentes { get; set; } = new List<string>();
    }
}
