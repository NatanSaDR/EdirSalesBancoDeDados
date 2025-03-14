namespace EdirSalesBancoDeDados.Domain
{
    public class Solicitacao : EntityBase
    {
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public string Observacao { get; set; }
        public string SEI { get; set; }
        public string Status { get; set; }
        public DateTime? DataFinalizado { get; set; }
        public ICollection<Oficio>? Oficios { get; set; } = new List<Oficio>();
        public ICollection<Grupo>? Grupos { get; set; } = new List<Grupo>(); // Grupo opcional
        public ICollection<Municipe>? Municipes { get; set; } = new List<Municipe>();
        public ICollection<Agente>? Agentes { get; set; } = new List<Agente>();
    }
}
