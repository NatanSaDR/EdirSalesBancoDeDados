namespace EdirSalesBancoDeDados.Domain
{
    public class Oficio
    {
        public int Id { get; set; }
        public string NumeroOficio { get; set; }
        public int SolicitacaoId { get; set; }

        public Solicitacao Solicitacao { get; set; }
    }
}
