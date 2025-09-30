using System.ComponentModel.DataAnnotations;
using Foodcore.Auth.Config;
using Foodcore.Auth.Exceptions;
using Foodcore.Auth.Helpers.Validation;
using Foodcore.Auth.Model;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace Foodcore.Auth.DTO
{
  /// <summary>
  /// DTO para criação de usuário.
  /// </summary>
  [OpenApiExample(typeof(UserCreateDTOExample))]
  public class UserCreateDTO
  {
    /// <summary>
    /// Nome completo do usuário.
    /// </summary>
    [OpenApiProperty(Description = "Nome completo do usuário.")]
    public string Name { get; set; } = "";

    /// <summary>
    /// E-mail do usuário.
    /// </summary>
    [OpenApiProperty(Description = "E-mail do usuário.")]
    [EmailAddress(ErrorMessage = "O Email informado é inválido.")]
    public string Email { get; set; } = "";

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    [OpenApiProperty(Description = "Senha do usuário.")]
    public string Password { get; set; } = "";

    /// <summary>
    /// CPF do usuário (somente dígitos).
    /// </summary>
    [OpenApiProperty(Description = "CPF do usuário (somente dígitos).")]
    [Cpf(ErrorMessage = "O CPF informado é inválido.")]
    public string Cpf { get; set; } = "";
  }
}