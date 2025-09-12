namespace Foodcore.Auth.Mapper
{
  using Foodcore.Auth.DTO;
  using Foodcore.Auth.Exceptions;
  using Foodcore.Auth.Model;
  using Foodcore.Auth.Model.ValueObjects;

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
      Cpf? cpfVo = null;
      Password? passwordVo = null;
      Email? emailVo = null;

      if (dto == null)
        throw new BusinessException("Dados do usuário inválidos.");

      if (!string.IsNullOrEmpty(dto.Password))
        passwordVo = new Password(dto.Password);

      if (!string.IsNullOrEmpty(dto.Cpf))
        cpfVo = new Cpf(dto.Cpf);

      if (!string.IsNullOrEmpty(dto.Email))
        emailVo = new Email(dto.Email);

      return new User(name: dto.Name, email: emailVo, password: passwordVo, cpf: cpfVo);
    }
  }
}