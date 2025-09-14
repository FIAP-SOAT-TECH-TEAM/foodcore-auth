using Amazon.CognitoIdentityProvider.Model;

namespace Foodcore.Auth.Presenter
{
  
  /// <summary>
  /// Fornece funcionalidades de apresentação relacionadas à autenticação.
  /// </summary>
  public static class AuthPresenter
  {

    /// <summary>
    /// Converte o resultado de autenticação do Amazon Cognito em um <see cref="DTO.AuthResponseDTO"/>.
    /// </summary>
    /// <param name="authenticationResultType">O resultado da autenticação retornado pelo Cognito.</param>
    /// <returns>Um <see cref="DTO.AuthResponseDTO"/> contendo os tokens de autenticação.</returns>
    public static DTO.AuthResponseDTO ToAuthResponseDTO(AuthenticationResultType authenticationResultType)
    {
      if (authenticationResultType == null) throw new ArgumentException(null, nameof(authenticationResultType));
      
      return new DTO.AuthResponseDTO
      {
        IdToken = authenticationResultType.IdToken!,
        AccessToken = authenticationResultType.AccessToken!,
        RefreshToken = authenticationResultType.RefreshToken!,
        ExpiresIn = (int)authenticationResultType.ExpiresIn!,
        TokenType = authenticationResultType.TokenType!
      };
    }
  }
}