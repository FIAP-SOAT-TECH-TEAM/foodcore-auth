using Foodcore.Auth.Exceptions;
using Foodcore.Auth.Model;

namespace Foodcore.Auth.Model
{
    /// <summary>
    /// Representa o usuário do sistema, contendo dados de identificação e credenciais.
    /// </summary>
    public class User
  {
    /// <summary>
    /// Nome completo do usuário.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Nome de usuário (login).
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// E-mail do usuário.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// CPF do usuário (somente dígitos).
    /// </summary>
    public required string Cpf { get; set; }

    /// <summary>
    /// Indica se o usuário deve ser tratado como cliente.
    /// </summary>
    /// <returns>
    /// true quando <see cref="Password"/> está vazia; caso contrário, false.
    /// </returns>
    public bool IsCustomer()
    {
      return string.IsNullOrEmpty(Password);
    }

    /// <summary>
    /// Obtém o perfil do usuário com base em <see cref="IsCustomer"/>.
    /// </summary>
    /// <returns>
    /// <see cref="Role.CUSTOMER"/> se <see cref="IsCustomer"/> for true; caso contrário, <see cref="Role.ADMIN"/>.
    /// </returns>
    public Role GetRole()
    {
      return IsCustomer() ? Role.CUSTOMER : Role.ADMIN;
    }

    /// <summary>
    /// Valida os campos obrigatórios para criação do usuário.
    /// </summary>
    /// <exception cref="BusinessException">
    /// Lançada quando os dados não atendem às regras.
    /// </exception>
    public void Validate()
    {
      bool hasEmailAndName = !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Name);
      bool hasOnlyCpf = !string.IsNullOrEmpty(Cpf) && string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Name);
      bool hasEmailNameCpf = !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Cpf);

      if (!hasEmailAndName && !hasOnlyCpf && !hasEmailNameCpf)
      {
        throw new BusinessException("Usuário inválido: deve ter Email e Nome, ou apenas CPF, ou ambos.");
      }
    }
  }
}