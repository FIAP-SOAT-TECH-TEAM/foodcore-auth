using Foodcore.Auth.Model;

namespace Foodcore.Auth.Services
{
    /// <summary>
    /// Serviço utilitário para obter a senha apropriada de um usuário.
    /// </summary>
    public static class UsuarioService
    {
        /// <summary>
        /// Obtém a senha do usuário. Se for cliente, retorna a senha padrão definida na variável de ambiente.
        /// </summary>
        /// <param name="user">Instância do usuário.</param>
        /// <returns>Senha a ser utilizada para autenticação.</returns>
        /// <exception cref="InvalidOperationException">
        /// Lançada quando a variável de ambiente DEFAULT_CUSTOMER_PASSWORD não está definida para usuários do tipo cliente.
        /// </exception>
        public static string GetUserPassword(User user)
        {
            if (user.IsCustomer())
            {
                return Environment.GetEnvironmentVariable("DEFAULT_CUSTOMER_PASSWORD") ?? throw new InvalidOperationException("Default customer password not set.");
            }
            return user.Password;
        }
    }
}