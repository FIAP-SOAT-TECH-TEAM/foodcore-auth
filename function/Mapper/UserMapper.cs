namespace Foodcore.Auth.Mapper
{
  using Foodcore.Auth.DTO;
  using Foodcore.Auth.Model;

  /// <summary>
  /// Fornece mapeamentos entre DTOs e modelos de domínio relacionados a usuário.
  /// </summary>
  public static class UserMapper
  {
    /// <summary>
    /// Converte um <see cref="UserCreateDTO"/> em um modelo de domínio <see cref="User"/>.
    /// </summary>
    /// <param name="dto">Objeto com os dados para criação do usuário.</param>
    /// <returns>Uma nova instância de <see cref="User"/> populada com os dados do DTO.</returns>
    public static User ToModel(UserCreateDTO dto)
    {
      return new User
      {
        Name = dto.Name,
        Username = dto.Username,
        Email = dto.Email,
        Password = dto.Password,
        Cpf = dto.Cpf
      };
    }
  }
}