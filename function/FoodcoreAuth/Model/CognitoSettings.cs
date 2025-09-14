namespace Foodcore.Auth.Model
{
  /// <summary>
  /// Representa as configurações do Amazon Cognito utilizadas pela aplicação.
  /// </summary>
  public class CognitoSettings
  {
    /// <summary>
    /// Identificador (ID) do User Pool do Cognito.
    /// </summary>
    public required string UserPoolId { get; set; }

    /// <summary>
    /// Identificador (ID) do App Client do Cognito.
    /// </summary>
    public required string AppClientId { get; set; }

    /// <summary>
    /// Região onde o Cognito está configurado (ex: us-east-1).
    /// </summary>
    public required string Region { get; set; }
  }
}