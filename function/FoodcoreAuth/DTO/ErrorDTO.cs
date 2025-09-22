using Foodcore.Auth.Config;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace Foodcore.Auth.DTO
{
  /// <summary>
  /// DTO para representar erros.
  /// </summary>
  [OpenApiExample(typeof(ErrorDTOExample))]
  public class ErrorDTO
  {
    /// <summary>
    /// Timestamp do erro.
    /// </summary>
    [OpenApiProperty(Description = "Timestamp do erro.")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Status do erro.
    /// </summary>
    [OpenApiProperty(Description = "Status do erro.")]
    public int Status { get; set; }

    /// <summary>
    /// Mensagem de erro.
    /// </summary>
    [OpenApiProperty(Description = "Mensagem de erro.")]
    public required string Message { get; set; } = "";

    /// <summary>
    /// Caminho onde o erro ocorreu.
    /// </summary>
    [OpenApiProperty(Description = "Caminho onde o erro ocorreu.")]
    public required string Path { get; set; }
  }
}