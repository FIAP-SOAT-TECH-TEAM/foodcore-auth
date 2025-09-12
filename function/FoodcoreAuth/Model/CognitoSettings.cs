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
    public string UserPoolId { get; set; } = string.Empty;
  }
}