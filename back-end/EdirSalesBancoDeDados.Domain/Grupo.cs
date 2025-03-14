namespace EdirSalesBancoDeDados.Domain
{
    public class Grupo : EntityBase
    {
        public string NomeGrupo { get; set; }
        public ICollection<Municipe>? Municipes { get; set; } = new List<Municipe>();
        public ICollection<Solicitacao>? Solicitacoes { get; set; } = new List<Solicitacao>();

    }
}
