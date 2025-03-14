namespace EdirSalesBancoDeDados.Domain.Interfaces
{
    public interface IMunicipeRepository
    {
        Task<int> CountAll();
        //import
        Task<int> ImportarMunicipes(List<Municipe> ListaMunicipes);
        //crud
        public Task<Municipe> Add(Municipe municipe);
        public Task<Municipe> GetById(int id);
        public Task<ICollection<Municipe>> List(int pagina = 1, int tamanhoPagina = 20);
        public Task Delete(Municipe municipe);
        public Task DeleteRange(IEnumerable<Municipe> municipes);
        public Task<Municipe> Update(Municipe municipe);
        public Task<List<Municipe>> Filtrar(
            string? nome,
            string? sexo,
            DateTime? aniversario,
            DateTime? aniversarioInicio,
            DateTime? aniversarioFim,
            string? logradouro,
            string? numero,
            string? complemento,
            string? bairro,
            string? cidade,
            string? estado,
            string? cep,
            string? observacao,
            string? email,
            string? telefone,
            string? grupo,
            int pagina = 1,
            int tamanhoPagina = 20
            );
    }
}
