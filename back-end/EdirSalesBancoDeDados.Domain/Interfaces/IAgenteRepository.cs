namespace EdirSalesBancoDeDados.Domain.Interfaces
{
    public interface IAgenteRepository
    {
        Task<int> CountAll();
        Task<int> ImportarAgentes(List<Agente> listaAgentes);
        Task<Agente> Add(Agente agente);
        Task<Agente> GetById(int id);
        Task<ICollection<Agente>> List(int pagina, int tamanhoPagina);
        Task Delete(Agente agente);
        Task<Agente> Update(Agente agente);
        Task<List<Agente>> Filtrar(
                int? id,
                string? agenteSolucao,
                string? contato,
                int pagina,
                int tamanhoPagina
                );
    }
}
