namespace Foodcore.Auth.Exceptions
{
  /// <summary>
  /// Exceção de regra de negócio.
  /// Utilize para indicar violações de regras de domínio.
  /// </summary>
  /// <remarks>
  /// Cria uma nova instância da exceção de negócio.
  /// </remarks>
  /// <param name="message">Mensagem descritiva do erro de negócio.</param>
  public class BusinessException(string message) : Exception(message)
    {
  }
}