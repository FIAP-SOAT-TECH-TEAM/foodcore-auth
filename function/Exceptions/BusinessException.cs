namespace Foodcore.Auth.Exceptions
{
    /// <summary>
    /// Exceção de regra de negócio.
    /// Utilize para indicar violações de regras de domínio.
    /// </summary>
    public class BusinessException : Exception
    {
        /// <summary>
        /// Cria uma nova instância da exceção de negócio.
        /// </summary>
        /// <param name="message">Mensagem descritiva do erro de negócio.</param>
        public BusinessException(string message) : base(message)
        {
        }
    }
}