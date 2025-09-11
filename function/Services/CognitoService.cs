using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Foodcore.Auth.Model;

namespace Foodcore.Auth.Services
{
  /// <summary>
  /// Serviço utilitário para operações no Amazon Cognito.
  /// </summary>
  public static class CognitoService
  {
    /// <summary>
    /// Lista usuários pelo e-mail ou CPF no User Pool.
    /// </summary>
    /// <param name="cognito">Cliente do Amazon Cognito.</param>
    /// <param name="settings">Configurações do Cognito (UserPoolId).</param>
    /// <param name="email">E-mail a pesquisar.</param>
    /// <param name="cpf">CPF a pesquisar.</param>
    /// <returns>Resposta com a lista de usuários.</returns>
    public static async Task<ListUsersResponse> ListUsersByEmailOrCpfAsync(
      IAmazonCognitoIdentityProvider cognito,
      CognitoSettings settings,
      string email,
      string cpf)
    {
      var listUsersRequest = new ListUsersRequest
      {
        UserPoolId = settings.UserPoolId,
        Filter = $"email = \"{email}\" or custom:cpf = \"{cpf}\""
      };
      
      return await cognito.ListUsersAsync(listUsersRequest);
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
        TemporaryPassword = Guid.NewGuid().ToString(),
        MessageAction = "SUPPRESS",
        UserAttributes =
          [
              new() { Name = "name", Value = user.Name },
              new() { Name = "email", Value = user.Email },
              new() { Name = "custom:cpf", Value = user.Cpf },
              new() { Name = "custom:role", Value = user.GetRole().ToString()},
              new() { Name = "custom:guest", Value = user.IsCustomer().ToString().ToLower() }
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

  }
}