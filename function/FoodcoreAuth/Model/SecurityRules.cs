namespace Foodcore.Auth.Model
{
    /// <summary>
    /// Representa uma regra de segurança para autorização de acesso a endpoints.
    /// </summary>
    public class SecurityRules
    {
        /// <summary>
        /// Método HTTP da solicitação.
        /// </summary>
        public required string HttpMethod { get; set; }
        /// <summary>
        /// Padrão de URL (regex) que a regra se aplica.
        /// </summary>
        public required string Pattern { get; set; }
        /// <summary>
        /// Conjunto de funções (roles) permitidas para acessar o endpoint. Se nulo, o endpoint é público.
        /// </summary>
        public HashSet<string>? AllowedRoles { get; set; }
    }
}