namespace Foodcore.Auth.Model
{
    /// <summary>
    /// Perfis de acesso disponíveis no sistema.
    /// </summary>
    /// <remarks>
    /// Define o nível de permissão para os usuários.
    /// </remarks>
    public enum Role
    {
        /// <summary>
        /// Perfil administrativo com acesso completo às funcionalidades.
        /// </summary>
        ADMIN,

        /// <summary>
        /// Perfil de cliente final com acesso limitado.
        /// </summary>
        CUSTOMER
    }
}