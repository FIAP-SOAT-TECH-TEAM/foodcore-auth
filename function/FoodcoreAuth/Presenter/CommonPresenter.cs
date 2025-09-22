using Foodcore.Auth.DTO;

namespace Foodcore.Auth.Presenter
{
  /// <summary>
  /// Fornece funcionalidades de apresentação comuns.
  /// </summary>
  public static class CommonPresenter
  {
    /// <summary>
    /// Converte uma exceção em um DTO de erro.
    /// </summary>
    /// <param name="ex">A exceção a ser convertida.</param>
    /// <param name="responseStatusCode">O código de status HTTP associado ao erro.</param>
    /// <param name="path">O caminho da requisição que causou o erro.</
    /// <returns>Uma instância de <see cref="ErrorDTO"/> representando o erro.</returns>
    public static ErrorDTO ToErrorDTO(Exception ex, int responseStatusCode, string path)
    {
      if (ex == null) throw new ArgumentException(null, nameof(ex));
      
      return new ErrorDTO
      {
        Status = responseStatusCode,
        Message = ex.Message,
        Path = path
      };

    }
  }

}