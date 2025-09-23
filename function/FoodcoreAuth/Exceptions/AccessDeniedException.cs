namespace Foodcore.Auth.Exceptions
{
    /// <summary>
    /// Exceção lançada quando o acesso a um recurso é negado.
    /// </summary>
    public class AccessDeniedException : Exception
    {
        /// <summary>
        /// Cria uma nova instância da exceção AccessDeniedException com a mensagem especificada.
        /// </summary>
        /// <param name="message">Mensagem descritiva do erro.</param>
        public AccessDeniedException(string message) : base(message) { }
    }
}