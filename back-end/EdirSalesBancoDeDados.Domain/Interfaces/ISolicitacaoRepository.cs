namespace EdirSalesBancoDeDados.Domain.Interfaces
{
    public interface ISolicitacaoRepository
    {
        Task<int> CountAll();
        Task<int> ImportarSolicitacoes(List<Solicitacao> listaSolicitacoes);
        Task<List<Solicitacao>> Filtrar(
            int? id,
            string? tipo,
            string? descricao,
            string? observacao,
            string? sei,
            string? status,
            DateTime? dataFinalizado,
            DateTime? dataFinalizadoInicio,
            DateTime? dataFinalizadoFim,
            DateTime? dataCadastroInicio,
            DateTime? dataCadastroFim,
            string? usuarioCadastro,
            string? usuarioAlteracao,
            string? grupo,
            string? agente,
            string? municipe,
            int pagina,
            int tamanhoPagina);
        Task<Solicitacao> Add(Solicitacao solicitacao);
        Task<Solicitacao> GetById(int id);
        Task<IEnumerable<Solicitacao>> List(int pagina, int tamanhoPagina);
        Task Delete(Solicitacao solicitacao);
        Task DeleteRange(IEnumerable<Solicitacao> solicitacoes);
        Task<Solicitacao> Update(Solicitacao solicitacao);
        Task<int?> GetSolicitacaoIdByOficio(string numeroOficio);
    }
}
