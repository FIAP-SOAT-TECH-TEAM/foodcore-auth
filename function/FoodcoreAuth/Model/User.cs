using Foodcore.Auth.Exceptions;
using Foodcore.Auth.Model.ValueObjects;

namespace Foodcore.Auth.Model
{
    /// <summary>
    /// Representa o usuário do sistema, contendo dados de identificação e credenciais.
    /// </summary>
    public class User
  {

    public User(string name, Email? email, Password? password, Cpf? cpf)
    {
      bool hasEmailAndName = email != null && !string.IsNullOrEmpty(name);
      bool hasOnlyCpf = cpf != null && email == null && string.IsNullOrEmpty(name);
      bool hasEmailNameCpf = email != null && !string.IsNullOrEmpty(name) && cpf != null;

      if (!hasEmailAndName && !hasOnlyCpf && !hasEmailNameCpf)
      {
        throw new BusinessException("Usuário inválido: deve ter Email e Nome, ou apenas CPF, ou ambos.");
      }

      if (!hasEmailNameCpf && password != null)
      {
        throw new BusinessException("Usuário inválido: ao informar uma senha, deve também informar Email, Nome e CPF.");
      }

      if (email == null)
      {
        var guid = Guid.NewGuid().ToString();

        email = new Email($"{guid}@foodcore.com");
        name = guid;
      }

      var username = email.Value.Split('@')[0];

      Name = name;
      Username = username;
      Email = email;
      Password = password;
      Cpf = cpf;
    }

    /// <summary>
    /// Nome completo do usuário.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Nome de usuário (login).
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// E-mail do usuário.
    /// </summary>
    public Email? Email { get; set; }

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    public Password? Password { get; set; }

    /// <summary>
    /// CPF do usuário (somente dígitos).
    /// </summary>
    public Cpf? Cpf { get; set; }

    /// <summary>
    /// Indica se o usuário deve ser tratado como cliente.
    /// </summary>
    /// <returns>
    /// true quando <see cref="Password"/> está vazia; caso contrário, false.
    /// </returns>
    public bool IsCustomer()
    {
      return Password == null;
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

  }
}