namespace EdirSalesBancoDeDados.Domain
{
    public class Telefone
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Numero { get; set; }
        public string Observacao { get; set; }

        public int MunicipeId { get; set; }
        public Municipe? Municipe { get; set; }
    }
}
