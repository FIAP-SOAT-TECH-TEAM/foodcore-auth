using Foodcore.Auth.Exceptions;

namespace Foodcore.Auth.Model.ValueObjects
{
  /// <summary>
  /// Representa uma senha de usuário.
  /// </summary>
  /// <remarks>
  /// A senha deve ter entre 8 e 20 caracteres e conter pelo menos:
  /// uma letra maiúscula, uma letra minúscula, um dígito e um caractere especial.
  /// O valor é armazenado exatamente como informado após a validação.
  /// </remarks>
  public class Password
  {
    /// <summary>
    /// Tamanho mínimo permitido para a senha.
    /// </summary>
    private const int MinLength = 8;

    /// <summary>
    /// Tamanho máximo permitido para a senha.
    /// </summary>
    private const int MaxLength = 20;

    /// <summary>
    /// Valor da senha após a validação.
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Cria uma nova instância de <see cref="Password"/> validando o valor informado.
    /// </summary>
    /// <param name="value">Senha informada.</param>
    /// <exception cref="BusinessException">
    /// Lançada quando a senha é nula/vazia, não respeita o tamanho mínimo/máximo
    /// ou não contém os tipos de caracteres exigidos.
    /// </exception>
    public Password(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new BusinessException("A senha não pode ser nula ou vazia.");
      }

      if (value.Length < MinLength || value.Length > MaxLength)
      {
        throw new BusinessException($"A senha deve ter entre {MinLength} e {MaxLength} caracteres.");
      }

      if (!HasUppercase(value) || !HasLowercase(value) || !HasDigit(value) || !HasSpecialCharacter(value))
      {
        throw new BusinessException("A senha deve conter pelo menos uma letra maiúscula, uma letra minúscula, um dígito e um caractere especial.");
      }

      Value = value;
    }

    /// <summary>
    /// Verifica se o valor contém pelo menos uma letra maiúscula.
    /// </summary>
    /// <param name="value">Texto a ser avaliado.</param>
    /// <returns>true se contiver pelo menos uma letra maiúscula; caso contrário, false.</returns>
    private static bool HasUppercase(string value) => value.Any(char.IsUpper);

    /// <summary>
    /// Verifica se o valor contém pelo menos uma letra minúscula.
    /// </summary>
    /// <param name="value">Texto a ser avaliado.</param>
    /// <returns>true se contiver pelo menos uma letra minúscula; caso contrário, false.</returns>
    private static bool HasLowercase(string value) => value.Any(char.IsLower);

    /// <summary>
    /// Verifica se o valor contém pelo menos um dígito numérico.
    /// </summary>
    /// <param name="value">Texto a ser avaliado.</param>
    /// <returns>true se contiver pelo menos um dígito; caso contrário, false.</returns>
    private static bool HasDigit(string value) => value.Any(char.IsDigit);

    /// <summary>
    /// Verifica se o valor contém pelo menos um caractere especial (não alfanumérico).
    /// </summary>
    /// <param name="value">Texto a ser avaliado.</param>
    /// <returns>true se contiver pelo menos um caractere especial; caso contrário, false.</returns>
    private static bool HasSpecialCharacter(string value) => value.Any(ch => !char.IsLetterOrDigit(ch));
  }
}