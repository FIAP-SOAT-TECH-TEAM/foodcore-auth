using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Foodcore.Auth.Model;
using Foodcore.Auth.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Foodcore.Auth.Services
{
  /// <summary>
  /// Serviço utilitário para operações no Amazon Cognito.
  /// </summary>
  public static class CognitoService
  {
    /// <summary>
    /// Obtém usuário pelo e-mail ou CPF no User Pool.
    /// </summary>
    /// <param name="cognito">Cliente do Amazon Cognito.</param>
    /// <param name="settings">Configurações do Cognito (UserPoolId).</param>
    /// <param name="email">E-mail a pesquisar.</param>
    /// <param name="cpf">CPF a pesquisar.</param>
    /// <returns>Resposta com o usuário encontrado.</returns>
    public static async Task<UserType?> GetUserByEmailOrCpfAsync(
      IAmazonCognitoIdentityProvider cognito,
      CognitoSettings settings,
      string email = "",
      string cpf = "")
    {
      cpf = cpf.Replace(".", "").Replace("-", "").Trim();
      cpf = string.IsNullOrEmpty(cpf) ? "none" : cpf;
      email = string.IsNullOrEmpty(email) ? "none" : email;

      // O filtro pode ser feito apenas por um atributo por vez
      var listUsersRequest = new ListUsersRequest
      {
        UserPoolId = settings.UserPoolId,
        Filter = $"email = \"{email}\""
      };
      var listUsers = await cognito.ListUsersAsync(listUsersRequest);

      if (listUsers.Users.Count == 0)
      {
        listUsersRequest = new ListUsersRequest
        {
          UserPoolId = settings.UserPoolId,
          Filter = $"preferred_username = \"{cpf}\""
        };
        listUsers = await cognito.ListUsersAsync(listUsersRequest);
      }

      var user = listUsers.Users.FirstOrDefault();

      return user;
    }

    /// <summary>
    /// Obtém usuário pelo sub (ID único) no User Pool.
    /// </summary>
    /// <param name="cognito">Cliente do Amazon Cognito.</param>
    /// <param name="settings">Configurações do Cognito (UserPoolId).</param>
    /// <param name="sub">Sub (ID único) do usuário.</param>
    /// <returns>Usuário encontrado ou nulo.</returns>
    public static async Task<UserType?> GetUserBySubAsync(
        IAmazonCognitoIdentityProvider cognito,
        CognitoSettings settings,
        string sub)
    {
      var listUsersRequest = new ListUsersRequest
      {
        UserPoolId = settings.UserPoolId,
        Filter = $"sub = \"{sub}\""
      };
      var listUsers = await cognito.ListUsersAsync(listUsersRequest);
      var user = listUsers.Users.FirstOrDefault();

      return user;
    }

    /// <summary>
    /// Cria um novo usuário no User Pool com senha temporária, suprimindo o envio de mensagens.
    /// </summary>
    /// <param name="cognito">Cliente do Amazon Cognito.</param>
    /// <param name="settings">Configurações do Cognito (UserPoolId).</param>
    /// <param name="user">Dados do usuário a ser criado.</param>
    /// <returns>Resposta da operação de criação.</returns>
    public static async Task<AdminCreateUserResponse> CreateUser(
        IAmazonCognitoIdentityProvider cognito,
        CognitoSettings settings,
        User user)
    {
      var adminCreateUserRequest = new AdminCreateUserRequest
      {
        UserPoolId = settings.UserPoolId,
        Username = user.Username,
        TemporaryPassword = PasswordUtils.GenerateTemporaryPassword(),
        MessageAction = "SUPPRESS",
        UserAttributes =
          [
              new() { Name = "name", Value = user.Name },
              new() { Name = "email", Value = user.Email?.Value ?? "" },
              // Apenas campos nativos podem ser pesquisados, pois são indexados
              // Escolhi este para ser armazenar o CPF, assim conseguimos pesquisar por ele
              new() { Name = "preferred_username", Value = user.Cpf?.Value ?? "" },
              new() { Name = "custom:cpf", Value = user.Cpf?.Value ?? "" },
              new() { Name = "custom:role", Value = user.GetRole().ToString()}
          ]
      };

      return await cognito.AdminCreateUserAsync(adminCreateUserRequest);
    }

    /// <summary>
    /// Define uma nova senha permanente para o usuário.
    /// </summary>
    /// <param name="cognito">Cliente do Amazon Cognito.</param>
    /// <param name="settings">Configurações do Cognito (UserPoolId).</param>
    /// <param name="username">Username no User Pool.</param>
    /// <param name="newPassword">Nova senha a ser definida.</param>
    /// <returns>Tarefa concluída quando a operação finalizar.</returns>
    public static async Task UpdateUserPassword(
        IAmazonCognitoIdentityProvider cognito,
        CognitoSettings settings,
        string username,
        string newPassword)
    {
      var setPasswordRequest = new AdminSetUserPasswordRequest
      {
        UserPoolId = settings.UserPoolId,
        Username = username,
        Password = newPassword,
        Permanent = true
      };

      await cognito.AdminSetUserPasswordAsync(setPasswordRequest);
    }


    /// <summary>
    /// Valida um access token JWT emitido pelo Amazon Cognito, utilizando JWKS (JSON Web Key Set).
    /// </summary>
    /// <param name="settings">
    /// Configurações do Cognito (Region, UserPoolId e AppClientId) usadas para validar o emissor e a audiência.
    /// </param>
    /// <param name="accessToken">JWT (access_token) a ser validado.</param>
    /// <returns>Instância de <see cref="JwtSecurityToken"/> já validada.</returns>
    /// <exception cref="HttpRequestException">
    /// Lançada quando não é possível obter as chaves públicas (JWKS) do Cognito.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Lançada quando o token é nulo, vazio ou possui formato inválido.
    /// </exception>
    /// <exception cref="SecurityTokenException">
    /// Lançada quando a validação do token falha (assinatura, emissor, audiência ou expiração).
    /// </exception>
    public static async Task<JwtSecurityToken> ValidateToken(CognitoSettings settings, string accessToken)
    {
      // Baixa as chaves públicas do Cognito (JWKS)
      using var http = new HttpClient();
      var jwksUri = $"https://cognito-idp.{settings.Region}.amazonaws.com/{settings.UserPoolId}/.well-known/jwks.json";
      var jwksJson = await http.GetStringAsync(jwksUri);
      var keys = new JsonWebKeySet(jwksJson).GetSigningKeys();

      var tokenHandler = new JwtSecurityTokenHandler();
      var validationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidIssuer = $"https://cognito-idp.{settings.Region}.amazonaws.com/{settings.UserPoolId}",
        ValidateAudience = false,
        // ValidAudience = settings.AppClientId,
        ValidateLifetime = true,
        RequireSignedTokens = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKeys = keys
      };

      tokenHandler.ValidateToken(accessToken, validationParameters, out var validatedToken);
      var jwtValidatedToken = (JwtSecurityToken)validatedToken;
      var clientId = jwtValidatedToken.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;

      // Access Tokens do Cognito não possuem o claim "aud", então validamos a audiência manualmente
      // https://stackoverflow.com/questions/53148711/why-doesnt-amazon-cognito-return-an-audience-field-in-its-access-tokens
      if (clientId != settings.AppClientId)
        throw new SecurityTokenException("Audience inválido.");

      return jwtValidatedToken;
    }

    /// <summary>
    /// Autentica um usuário no User Pool utilizando o fluxo ADMIN_USER_PASSWORD_AUTH.
    /// </summary>
    /// <param name="cognito">Cliente do Amazon Cognito.</param>
    /// <param name="settings">Configurações do Cognito.</param>
    /// <param name="username">Nome de usuário (username) do usuário a ser autenticado.</param>
    /// <param name="password">Senha do usuário a ser autenticado.</param>
    /// <returns>Resultado da autenticação, incluindo tokens se bem-sucedida.</returns>
    /// <exception cref="NotAuthorizedException">Lançada quando as credenciais são inválidas.</exception>
    /// <exception cref="UserNotFoundException">Lançada quando o usuário não é encontrado.</exception>
    public static async Task<AuthenticationResultType> AuthenticateUserAsync(
            IAmazonCognitoIdentityProvider cognito,
            CognitoSettings settings,
            string username,
            string password)
    {
      var authRequest = new AdminInitiateAuthRequest
      {
        UserPoolId = settings.UserPoolId,
        ClientId = settings.AppClientId,
        AuthFlow = AuthFlowType.ADMIN_USER_PASSWORD_AUTH,
        AuthParameters = new Dictionary<string, string>
        {
            { "USERNAME", username },
            { "PASSWORD", password }
        }
      };

      var authResponse = await cognito.AdminInitiateAuthAsync(authRequest);

      return authResponse.AuthenticationResult;
    }
  }
}