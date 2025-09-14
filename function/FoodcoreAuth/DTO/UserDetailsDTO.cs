namespace Foodcore.Auth.DTO
{
  /// <summary>
  /// Data Transfer Object (DTO) para detalhes do usuário.
  /// </summary>
  public class UserDetailsDTO
  {
    /// <summary>
    /// Identificador único do usuário.
    /// </summary>
    public required string Subject { get; set; }
    /// <summary>
    /// Nome completo do usuário.
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// E-mail do usuário.
    /// </summary>
    public required string Email { get; set; }
    /// <summary>
    /// CPF do usuário.
    /// </summary>
    public required string Cpf { get; set; }
    /// <summary>
    /// Papel (role) do usuário.
    /// </summary>
    public required string Role { get; set; }
  }
}