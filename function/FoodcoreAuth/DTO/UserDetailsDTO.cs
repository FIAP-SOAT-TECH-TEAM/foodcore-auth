using Foodcore.Auth.Config;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace Foodcore.Auth.DTO
{
  /// <summary>
  /// Data Transfer Object (DTO) para detalhes do usuário.
  /// </summary>
  [OpenApiExample(typeof(UserDetailsDTOExample))]
  public class UserDetailsDTO
  {
    /// <summary>
    /// Identificador único do usuário.
    /// </summary>
    [OpenApiProperty(Description = "Identificador único do usuário.")]
    public required string Subject { get; set; }

    /// <summary>
    /// Nome completo do usuário.
    /// </summary>
    [OpenApiProperty(Description = "Nome completo do usuário.")]
    public required string Name { get; set; }

    /// <summary>
    /// E-mail do usuário.
    /// </summary>
    [OpenApiProperty(Description = "E-mail do usuário.")]
    public required string Email { get; set; }

    /// <summary>
    /// CPF do usuário.
    /// </summary>
    [OpenApiProperty(Description = "CPF do usuário.")]
    public required string Cpf { get; set; }

    /// <summary>
    /// Papel (role) do usuário.
    /// </summary>
    [OpenApiProperty(Description = "Papel (role) do usuário.")]
    public required string Role { get; set; }
  }
}