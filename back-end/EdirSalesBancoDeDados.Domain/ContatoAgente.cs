namespace EdirSalesBancoDeDados.Domain
{
    public class ContatoAgente : EntityBase
    {
        public string Contato { get; set; }

        public int AgenteId { get; set; }
        public Agente? Agente { get; set; }
    }
}
