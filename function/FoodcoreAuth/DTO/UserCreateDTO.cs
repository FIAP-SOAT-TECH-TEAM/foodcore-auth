using Foodcore.Auth.Exceptions;
using Foodcore.Auth.Model;

namespace Foodcore.Auth.DTO
{
  /// <summary>
  /// DTO para criação de usuário.
  /// </summary>
  public class UserCreateDTO
  {
    /// <summary>
    /// Nome completo do usuário.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// E-mail do usuário.
    /// </summary>
    public string Email { get; set; } = "";

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    public string Password { get; set; } = "";

    /// <summary>
    /// CPF do usuário (somente dígitos).
    /// </summary>
    public string Cpf { get; set; } = "";

  }
}