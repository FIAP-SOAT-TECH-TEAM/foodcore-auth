using Foodcore.Auth.Exceptions;

namespace Foodcore.Auth.Model.ValueObjects
{
  /// <summary>
  /// Representa um endereço de e-mail válido como um Value Object.
  /// </summary>
  public class Email
  {
    /// <summary>
    /// Valor do endereço de e-mail.
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Cria uma nova instância de <see cref="Email"/> validando o formato básico.
    /// </summary>
    /// <param name="value">Endereço de e-mail a ser atribuído.</param>
    /// <exception cref="BusinessException">Lançada quando o e-mail é nulo, vazio, em branco ou não contém '@'.</exception>
    public Email(string value)
    {
      if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
      {
        throw new BusinessException("O Email informado é inválido.");
      }
      Value = value;
    }
  }
}