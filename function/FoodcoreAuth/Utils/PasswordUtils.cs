using System.Security.Cryptography;
using System.Text;

namespace Foodcore.Auth.Utils
{
  /// <summary>
  /// Utilitários para geração de senhas com requisitos de complexidade.
  /// </summary>
  public static class PasswordUtils
  {
    /// <summary>
    /// Gera uma senha temporária aleatória com base nos requisitos informados.
    /// </summary>
    /// <param name="length">Comprimento da senha a ser gerada. Deve ser maior ou igual ao número de categorias exigidas (minúsculas, maiúsculas, números, símbolos).</param>
    /// <param name="requireLowercase">Se true, exige ao menos uma letra minúscula.</param>
    /// <param name="requireUppercase">Se true, exige ao menos uma letra maiúscula.</param>
    /// <param name="requireNumbers">Se true, exige ao menos um dígito.</param>
    /// <param name="requireSymbols">Se true, exige ao menos um símbolo.</param>
    /// <returns>Senha gerada atendendo aos requisitos especificados.</returns>
    /// <exception cref="ArgumentException">Lançada quando nenhuma categoria de caracteres é habilitada.</exception>
    /// <remarks>
    /// - Usa <see cref="RandomNumberGenerator"/> para garantir aleatoriedade.<br/>
    /// - A senha final é embaralhada para evitar previsibilidade na posição dos caracteres obrigatórios.<br/>
    /// - Garanta que <paramref name="length"/> seja suficiente para cobrir todas as categorias obrigatórias selecionadas.
    /// </remarks>
    public static string GenerateTemporaryPassword(int length = 8,
          bool requireLowercase = true,
          bool requireUppercase = true,
          bool requireNumbers = true,
          bool requireSymbols = true)
    {

      const string lowers = "abcdefghijklmnopqrstuvwxyz";
      const string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      const string numbers = "0123456789";
      const string symbols = "!@#$%^&*()-_=+[]{};:,.<>?";

      var charsToGeneratePassword = new StringBuilder();

      if (requireLowercase) charsToGeneratePassword.Append(lowers);
      if (requireUppercase) charsToGeneratePassword.Append(uppers);
      if (requireNumbers) charsToGeneratePassword.Append(numbers);
      if (requireSymbols) charsToGeneratePassword.Append(symbols);

      if (charsToGeneratePassword.Length == 0)
        throw new ArgumentException("No character sets defined.");

      var password = new char[length];

      int index = 0;

      if (requireLowercase) password[index++] = GetRandomChar(lowers);
      if (requireUppercase) password[index++] = GetRandomChar(uppers);
      if (requireNumbers) password[index++] = GetRandomChar(numbers);
      if (requireSymbols) password[index++] = GetRandomChar(symbols);

      for (int i = index; i < length; i++)
        password[i] = GetRandomChar(charsToGeneratePassword.ToString());

      password = [.. password.OrderBy(_ => RandomNumber())];

      var passwordValue = new string(password);

      return passwordValue!;
    }

    /// <summary>
    /// Obtém um caractere aleatório de uma cadeia de caracteres fornecida.
    /// </summary>
    /// <param name="chars">Conjunto de caracteres possíveis.</param>
    /// <returns>Um caractere escolhido aleatoriamente do conjunto.</returns>
    private static char GetRandomChar(string chars)
    {
      int index = RandomNumber(0, chars.Length);

      return chars[index];
    }

    /// <summary>
    /// Gera um número inteiro aleatório criptograficamente seguro no intervalo especificado.
    /// </summary>
    /// <param name="min">Valor mínimo inclusivo.</param>
    /// <param name="max">Valor máximo exclusivo.</param>
    /// <returns>Número inteiro aleatório no intervalo [min, max).</returns>
    private static int RandomNumber(int min = int.MinValue, int max = int.MaxValue)
    {
      return RandomNumberGenerator.GetInt32(min, max);
    }
  }
}