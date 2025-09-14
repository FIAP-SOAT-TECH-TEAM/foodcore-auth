namespace Foodcore.Auth.DTO
{
  /// <summary>
  /// Data Transfer Object (DTO) para resposta de autenticação.
  /// </summary>
  public class AuthResponseDTO
  {
    /// <summary>
    /// Token de ID (ID Token) JWT.
    /// </summary>
    public required string IdToken { get; set; }
    /// <summary>
    /// Token de acesso (Access Token) JWT.
    /// </summary>
    public required string AccessToken { get; set; }

    /// <summary>
    /// Token de atualização (Refresh Token) JWT.
    /// </summary>
    public required string RefreshToken { get; set; }
    /// <summary>
    /// Tempo de expiração do token.
    /// </summary>
    public required int ExpiresIn { get; set; }
    /// <summary>
    /// Tipo de token.
    /// </summary>
    public required string TokenType { get; set; }
  }
}