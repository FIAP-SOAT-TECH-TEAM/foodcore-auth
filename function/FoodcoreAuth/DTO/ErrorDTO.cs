namespace Foodcore.Auth.DTO
{
  /// <summary>
  /// DTO para representar erros.
  /// </summary>
  public class ErrorDTO
  {
    /// <summary>
    /// Timestamp do erro.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Mensagem de erro.
    /// </summary>
    public required string Message { get; set; } = "";
  }
}