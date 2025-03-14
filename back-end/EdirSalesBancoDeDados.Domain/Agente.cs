namespace EdirSalesBancoDeDados.Domain
{
    public class Agente : EntityBase
    {
        public string AgenteSolucao { get; set; }
        public ICollection<ContatoAgente>? Contatos { get; set; } = new List<ContatoAgente>();
        public ICollection<Solicitacao>? Solicitacoes { get; set; } = new List<Solicitacao>();
    }
}
