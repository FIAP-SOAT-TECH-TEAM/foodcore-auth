namespace Foodcore.Auth.DTO
{
  /// <summary>
  /// Data Transfer Object (DTO) para autenticação do cliente.
  /// </summary>
  public class CustomerAuthDTO
  {
    /// <summary>
    /// E-mail do cliente.
    /// </summary>
    public string Email { get; set; } = "";
    /// <summary>
    /// CPF do cliente.
    /// </summary>
    public string Cpf { get; set; } = "";
  }
}