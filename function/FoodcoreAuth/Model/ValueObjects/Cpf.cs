using Foodcore.Auth.Exceptions;

namespace Foodcore.Auth.Model.ValueObjects
{
  /// <summary>
  /// Representa um CPF (Cadastro de Pessoas Físicas).
  /// </summary>
  /// <remarks>
  /// O valor é armazenado com exatamente 11 dígitos numéricos, sem pontuação.
  /// </remarks>
  public class Cpf
  {
    /// <summary>
    /// Valor do CPF contendo apenas dígitos (11 caracteres).
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Cria uma nova instância de <see cref="Cpf"/> validando e normalizando o valor informado.
    /// </summary>
    /// <param name="value">CPF informado, com ou sem pontuação.</param>
    /// <exception cref="BusinessException">
    /// Lançada quando o CPF é nulo/vazio ou não contém exatamente 11 dígitos numéricos após a limpeza.
    /// </exception>
    public Cpf(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new BusinessException("O CPF não pode ser nulo ou vazio.");
      }

      var isValid = Utils.CpfUtils.IsCpf(value);

      if (!isValid)
      {
        throw new BusinessException("O CPF informado é inválido.");
      }

      var cleanedCpfValue = Utils.CpfUtils.CleanCpf(value);

      Value = cleanedCpfValue;
    }

  }
}