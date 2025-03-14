using EdirSalesBancoDeDados.Application.Interfaces;
using EdirSalesBancoDeDados.Domain;
using EdirSalesBancoDeDados.Domain.Interfaces;

namespace EdirSalesBancoDeDados.Application.UseCases
{
    public class UserUseCase : IUserUseCase
    {
        private readonly IUserRepository _userRepository;

        public UserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var usernameClean = username.Trim();
            var user = await _userRepository.GetByUsername(usernameClean);
            if (user == null || !user.VerificarSenha(password))
            {
                return null;
            }
            return user;
        }
        public async Task Add(string username, string password, string role)
        {
            var newUser = new User(username, password, role);
            await _userRepository.Add(newUser);
        }

        public async Task Delete(int id)
        {
            await _userRepository.Delete(id);
        }

        public async Task<User?> GetById(int id)
        {
            return await _userRepository.GetById(id);
        }

        public async Task<List<User>> List()
        {
            return await _userRepository.List();
        }

        public async Task Update(int id, string username, string password, string role)
        {
            var user = await _userRepository.GetById(id);
            if (user != null)
            {
                user.Username = username;
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                user.Role = role;
                await _userRepository.Update(user);
            }
        }

        public async Task<int> CountAll()
        {
            return await _userRepository.CountAll();
        }
        public async Task<User?> GetUserByUsername(string username)
        {
            return await _userRepository.GetByUsername(username);
        }
    }
}
