namespace Foodcore.Auth.DTO
{
    /// <summary>
    /// DTO de resposta para a criação de usuário.
    /// </summary>
    /// <remarks>
    /// Contém a mensagem do resultado e o identificador único (sub) do usuário criado.
    /// </remarks>
    public class UserCreatedResponseDTO
    {
        /// <summary>
        /// Mensagem descritiva do resultado da operação.
        /// </summary>
        /// <example>Usuário criado com sucesso.</example>
        public required string Message { get; set; }

        /// <summary>
        /// Identificador único (sub) do usuário criado no provedor de identidade.
        /// </summary>
        /// <example>c1a2b3c4-d5e6-7890-abcd-ef1234567890</example>
        public required string UserSub { get; set; }
    }
}