using Foodcore.Auth.Config;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace Foodcore.Auth.DTO
{
    /// <summary>
    /// DTO de resposta para a criação de usuário.
    /// </summary>
    /// <remarks>
    /// Contém a mensagem do resultado e o identificador único (sub) do usuário criado.
    /// </remarks>
    [OpenApiExample(typeof(UserCreatedResponseDTOExample))]
    public class UserCreatedResponseDTO
    {
        /// <summary>
        /// Mensagem descritiva do resultado da operação.
        /// </summary>
        [OpenApiProperty(Description = "Mensagem descritiva do resultado da operação.")]
        public required string Message { get; set; }

        /// <summary>
        /// Identificador único (sub) do usuário criado no provedor de identidade.
        /// </summary>
        [OpenApiProperty(Description = "Identificador único (sub) do usuário criado no provedor de identidade.")]
        public required string UserSub { get; set; }
    }
}