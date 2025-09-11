using Amazon.CognitoIdentityProvider.Model;
using Foodcore.Auth.DTO;

namespace Foodcore.Auth.Presenter
{
    /// <summary>
    /// Fornece mapeamentos de respostas do Amazon Cognito para DTOs de usuário.
    /// </summary>
    public static class UserPresenter
    {
        /// <summary>
        /// Converte a resposta de criação de usuário do Amazon Cognito em um <see cref="UserCreatedResponseDTO"/>.
        /// </summary>
        /// <param name="user">Resposta do Cognito retornada por <c>AdminCreateUser</c>.</param>
        /// <returns>
        /// Um <see cref="UserCreatedResponseDTO"/> com a mensagem de sucesso e o identificador (<c>sub</c>) do usuário criado.
        /// </returns>
        /// <exception cref="InvalidOperationException">Lançada quando <paramref name="user"/> é nulo.</exception>
        public static UserCreatedResponseDTO ToUserCreatedResponse(AdminCreateUserResponse user)
        {
            if (user == null) throw new ArgumentException(null, nameof(user));

            return new UserCreatedResponseDTO
            {
                Message = "Usuário criado com sucesso!",
                UserSub = user.User?.Attributes?.Find(attr => attr.Name == "sub")?.Value ?? string.Empty
            };
        }
    }
}