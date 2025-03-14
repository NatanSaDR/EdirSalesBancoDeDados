namespace EdirSalesBancoDeDados.Application.DTOs.ViewDetails
{
    public class DetalheGrupoDto
    {
        public int IdGrupo { get; set; }
        public string NomeGrupo { get; set; }
        public List<DetalheMunicipeGrupo> Municipes { get; set; } = new List<DetalheMunicipeGrupo>();
    }
}
