using System.Text.RegularExpressions;
using Amazon.CognitoIdentityProvider.Model;
using Foodcore.Auth.Exceptions;
using Foodcore.Auth.Helpers;
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
        /// <param name="settings">Configurações do Cognito.</param>
        /// <param name="user">Instância do usuário.</param>
        /// <returns>Senha a ser utilizada para autenticação.</returns>
        /// <exception cref="BusinessException">Lançada quando a senha do usuário não está definida.</exception>
        public static string GetUserPassword(CognitoSettings settings, User user)
        {
            if (user.IsCustomer())    
                return settings.DefaultCustomerPassword;
            
            var password = user.Password;
            if (password == null || string.IsNullOrWhiteSpace(password.Value))
                throw new BusinessException("Senha do usuário não definida.");
            
            return password.Value;
        }
        /// <summary>
        /// Verifica se o usuário com a função especificada pode acessar a URL dada com o método HTTP fornecido.
        /// </summary>
        /// <param name="url">URL que o usuário está tentando acessar.</param>
        /// <param name="userRole">Função do usuário (Role).</param>
        /// <param name="httpMethod">Método HTTP da solicitação.</param>
        /// <exception cref="AccessDeniedException">Lançada quando o usuário não tem permissão para acessar a URL, ou quando a URL não é fornecida.</exception>
        /// <exception cref="BusinessException">Lançada quando a URL ou o método HTTP não são fornecidos.</exception>
        /// <returns></returns>
        public static void UserCanAccessUrl(string url, string httpMethod, Role? userRole = null)
        {
            // Implicit Deny
            var canAccess = false;

            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(httpMethod))
                throw new BusinessException("URL ou método HTTP não fornecidos.");

            httpMethod = httpMethod.ToUpperInvariant();
            var authorizationRules = AuthorizationHelper.Rules;

            foreach (var rule in authorizationRules)
            {
                if (rule.HttpMethod != "*" && rule.HttpMethod != httpMethod)
                    continue;

                if (!Regex.IsMatch(url, rule.Pattern))
                    continue;

                if (rule.AllowedRoles == null)
                    return;
                
                canAccess = rule.AllowedRoles.Contains(userRole?.ToString() ?? "");
            }
            
            if (!canAccess)
                throw new AccessDeniedException($"Você não tem permissão para acessar este recurso.");
        }
    }
}