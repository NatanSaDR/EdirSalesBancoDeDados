namespace EdirSalesBancoDeDados.Domain.Interfaces
{
    public interface IGrupoRepository
    {
        Task<int> CountAll();
        //import
        Task<int> ImportarGrupos(List<Grupo> listaGrupo);
        //crud
        Task<Grupo> Add(Grupo grupo);
        Task<Grupo> GetById(int id);
        Task<IEnumerable<Grupo>> List(int pagina, int tamanhoPagina);
        Task Delete(Grupo grupo);
        Task<Grupo> Update(Grupo grupo);
        Task<Grupo> DetalhesDoGrupo(int id);
        Task<List<Grupo>> Filtrar(int? id, string? nome, int pagina, int tamanhoPagina);
    }
}
