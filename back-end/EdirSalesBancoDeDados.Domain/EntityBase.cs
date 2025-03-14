namespace EdirSalesBancoDeDados.Domain
{
    public class EntityBase
    {
        public int Id { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string UsuarioCadastro { get; set; }
        public string? UsuarioAlteracao { get; set; } 
    }
}
