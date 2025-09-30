using System.ComponentModel.DataAnnotations;
using Foodcore.Auth.Config;
using Foodcore.Auth.Helpers.Validation;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace Foodcore.Auth.DTO
{
  /// <summary>
  /// Data Transfer Object (DTO) para autenticação do cliente.
  /// </summary>
  [OpenApiExample(typeof(CustomerAuthDTOExample))]
  public class CustomerAuthDTO
  {
    /// <summary>
    /// E-mail do cliente.
    /// </summary>
    [OpenApiProperty(Description = "E-mail do cliente.")]
    [EmailAddress(ErrorMessage = "O Email informado é inválido.")]
    public string Email { get; set; } = "";

    /// <summary>
    /// CPF do cliente.
    /// </summary>
    [OpenApiProperty(Description = "CPF do cliente.")]
    [Cpf(ErrorMessage = "O CPF informado é inválido.")]
    public string Cpf { get; set; } = "";
  }
}