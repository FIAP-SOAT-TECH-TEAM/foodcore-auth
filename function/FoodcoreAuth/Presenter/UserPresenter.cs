using System.Security.Claims;
using Amazon.CognitoIdentityProvider.Model;
using Foodcore.Auth.DTO;
using Foodcore.Auth.Exceptions;

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
                UserSub = user.User.Attributes.Find(attr => attr.Name == "sub")!.Value!
            };
        }

        /// <summary>
        /// Converte a resposta de obtenção de usuário do Amazon Cognito em um <see cref="UserDetailsDTO"/>.
        /// </summary>
        /// <param name="user">Resposta do Cognito retornada por <c>AdminGetUser</c>.</param>
        /// <param name="claims">Coleção de claims extraídas do token JWT do usuário.</param>
        /// <returns>Um <see cref="UserDetailsDTO"/> com os detalhes do usuário.</returns>
        /// <exception cref="InvalidOperationException">Lançada quando <paramref name="user"/> é nulo.</exception>
        public static UserDetailsDTO ToUserDetailsDTO(UserType? user = null, IEnumerable<Claim>? claims = null)
        {
            if (user == null) throw new ArgumentException(null, nameof(user));
            
            if (user.UserCreateDate == null)
                throw new BusinessException("A data de criação do usuário não pode ser nula.");

            return new UserDetailsDTO
            {
                Subject = claims?.FirstOrDefault(c => c.Type == "sub")?.Value ?? "",
                Name = user?.Attributes.Find(attr => attr.Name == "name")?.Value ?? "",
                Email = user?.Attributes.Find(attr => attr.Name == "email")?.Value ?? "",
                Cpf = user?.Attributes.Find(attr => attr.Name == "custom:cpf")?.Value ?? "",
                Role = user?.Attributes.Find(attr => attr.Name == "custom:role")?.Value ?? "",
                CreatedAt = user?.UserCreateDate.GetValueOrDefault(DateTime.MinValue) ?? DateTime.MinValue
            };
        }
    }
}