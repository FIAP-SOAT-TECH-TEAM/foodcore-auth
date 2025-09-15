using Foodcore.Auth.Config;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace Foodcore.Auth.DTO
{
    /// <summary>
    /// Data Transfer Object (DTO) para resposta de autenticação.
    /// </summary>
    [OpenApiExample(typeof(AuthResponseDTOExample))]
    public class AuthResponseDTO
    {
        /// <summary>
        /// Token JWT que representa a identidade do usuário.
        /// </summary>
        [OpenApiProperty(Description = "Token JWT que representa a identidade do usuário.")]
        public required string IdToken { get; set; }

        /// <summary>
        /// Token JWT usado para autenticação nas APIs.
        /// </summary>
        [OpenApiProperty(Description = "Token JWT usado para autenticação nas APIs.")]
        public required string AccessToken { get; set; }

        /// <summary>
        /// Token usado para renovar o Access Token sem refazer login.
        /// </summary>
        [OpenApiProperty(Description = "Token usado para renovar o Access Token sem refazer login.")]
        public required string RefreshToken { get; set; }

        /// <summary>
        /// Tempo de expiração do token (em segundos).
        /// </summary>
        [OpenApiProperty(Description = "Tempo de expiração do token (em segundos).")]
        public required int ExpiresIn { get; set; }

        /// <summary>
        /// Tipo do token emitido, geralmente 'Bearer'.
        /// </summary>
        [OpenApiProperty(Description = "Tipo do token emitido, geralmente 'Bearer'.")]
        public required string TokenType { get; set; }
    }
}