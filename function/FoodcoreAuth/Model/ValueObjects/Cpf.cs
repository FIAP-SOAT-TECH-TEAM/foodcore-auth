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

      var cleanedCpfValue = CleanCpf(value);

      if (cleanedCpfValue.Length != 11 || !cleanedCpfValue.All(char.IsDigit))
      {
        throw new BusinessException("O CPF deve conter exatamente 11 dígitos numéricos.");
      }

      Value = cleanedCpfValue;
    }

    /// <summary>
    /// Remove pontuação (pontos e traços) e espaços do CPF.
    /// </summary>
    /// <param name="cpf">CPF a ser limpo.</param>
    /// <returns>String contendo apenas os dígitos do CPF.</returns>
    private static string CleanCpf(string cpf)
    {
      return cpf.Replace(".", "").Replace("-", "").Trim();
    }

  }
}