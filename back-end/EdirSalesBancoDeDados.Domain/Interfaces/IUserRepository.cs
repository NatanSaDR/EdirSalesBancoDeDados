namespace EdirSalesBancoDeDados.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<int> CountAll();
        Task<User?> GetById(int id);
        Task<List<User>> List();
        Task Add(User user);
        Task Update(User user);
        Task Delete(int id);
        Task<User?> GetByUsername(string username);
    }
}
