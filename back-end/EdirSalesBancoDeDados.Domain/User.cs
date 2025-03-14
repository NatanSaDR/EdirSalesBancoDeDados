using System.ComponentModel.DataAnnotations;

namespace EdirSalesBancoDeDados.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Exemplo: "Admin", "User", etc.

        public User()
        {
        }

        public User(string username, string password, string role)
        {
            Username = username.ToLower();
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            Role = role;
        }

        public bool VerificarSenha(string senha)
        {
            return BCrypt.Net.BCrypt.Verify(senha, PasswordHash);
        }
    }
}
