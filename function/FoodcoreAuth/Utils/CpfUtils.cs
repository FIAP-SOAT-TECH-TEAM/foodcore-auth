namespace Foodcore.Auth.Utils
{
  /// <summary>
  /// Utilitário para validação de CPF.
  /// </summary>
  public static class CpfUtils
  {
    /// <summary>
    /// Verifica se o CPF é válido. Referência: <see href="https://gist.github.com/rdakar/dba890b5e2cbdeb7c62c0dee9f627a7f"/>
    /// </summary>
    /// <param name="cpf">Número do CPF a ser validado.</param>
    /// <returns>True se o CPF for válido, caso contrário, false.</returns>
    public static bool IsCpf(string cpf)
    {
      int[] multiplicador1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
      int[] multiplicador2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

      cpf = CleanCpf(cpf);

      if (cpf.Length != 11)
        return false;

      for (int j = 0; j < 10; j++)
        if (j.ToString().PadLeft(11, char.Parse(j.ToString())) == cpf)
          return false;

      string tempCpf = cpf[..9];
      int soma = 0;

      for (int i = 0; i < 9; i++)
        soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

      int resto = soma % 11;
      if (resto < 2)
        resto = 0;
      else
        resto = 11 - resto;

      string digito = resto.ToString();
      tempCpf += digito;
      soma = 0;
      for (int i = 0; i < 10; i++)
        soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

      resto = soma % 11;
      if (resto < 2)
        resto = 0;
      else
        resto = 11 - resto;

      digito += resto.ToString();

      return cpf.EndsWith(digito);
    }
    
    /// <summary>
    /// Remove pontuação (pontos e traços) e espaços do CPF.
    /// </summary>
    /// <param name="cpf">Número do CPF a ser limpo.</param>
    /// <returns>CPF limpo, contendo apenas dígitos.</returns>
    public static string CleanCpf(string cpf)
    {
      return cpf.Trim().Replace(".", "").Replace("-", "");
    }
  }
}