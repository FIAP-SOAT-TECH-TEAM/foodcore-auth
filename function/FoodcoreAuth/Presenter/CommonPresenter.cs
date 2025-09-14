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
    /// <returns>Uma instância de <see cref="ErrorDTO"/> representando o erro.</returns>
    public static ErrorDTO ToErrorDTO(Exception ex)
    {
      if (ex == null) throw new ArgumentException(null, nameof(ex));
      
      return new ErrorDTO
      {
        Message = ex.Message
      };

    }
  }

}