namespace EdirSalesBancoDeDados.Application.DTOs
{
    public class GrupoDto
    {
        public int Id { get; set; }
        public string NomeGrupo { get; set; }
        public string DataCadastro { get; set; }
        public string? DataAlteracao { get; set; }
        public string UsuarioCadastro { get; set; }
        public string? UsuarioAlteracao { get; set; }
    }
}
