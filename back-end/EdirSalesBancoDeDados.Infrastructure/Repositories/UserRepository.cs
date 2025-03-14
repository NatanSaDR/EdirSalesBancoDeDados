using EdirSalesBancoDeDados.Domain.Interfaces;
using EdirSalesBancoDeDados.Domain;
using Microsoft.EntityFrameworkCore;

namespace EdirSalesBancoDeDados.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EdirSalesContext _context;

        public UserRepository(EdirSalesContext context)
        {
            _context = context;
        }

        public async Task<User?> GetById(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<User?> GetByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<List<User>> List()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task Add(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAll()
        {
            return await _context.Users.CountAsync();
        }

        public async Task Delete(int id)
        {
            var user = await GetById(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
