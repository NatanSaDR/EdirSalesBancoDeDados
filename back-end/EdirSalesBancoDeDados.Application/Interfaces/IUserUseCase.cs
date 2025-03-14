using EdirSalesBancoDeDados.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdirSalesBancoDeDados.Application.Interfaces
{
    public interface IUserUseCase
    {
        Task<int> CountAll();

        Task<User?> GetById(int id);
        Task<List<User>> List();
        Task Add(string username, string password, string role);
        Task Update(int id, string username, string password, string role);
        Task Delete(int id);
        Task<User?> GetUserByUsername(string username);
        Task<User?> AuthenticateAsync(string username, string password);
    }
}
