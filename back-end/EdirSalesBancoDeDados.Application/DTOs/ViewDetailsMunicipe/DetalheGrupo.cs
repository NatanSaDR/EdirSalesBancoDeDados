using EdirSalesBancoDeDados.Domain;

namespace EdirSalesBancoDeDados.Application.DTOs.ViewDetailsMunicipe
{
    public class DetalheGrupo
    {
        public int Id { get; set; }
        public string NomeGrupo { get; set; }
        public string DataCadastro { get; set; }
        public string? DataAlteracao { get; set; }
        public string UsuarioCadastro { get; set; }
        public string? UsuarioAlteracao { get; set; }
    }
}
