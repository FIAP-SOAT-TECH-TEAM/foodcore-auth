namespace Foodcore.Auth.Helpers.Validation
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.Linq;
  using Foodcore.Auth.Utils;

  /// <summary>
  /// Validador de CPF.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
  public class CpfAttribute : ValidationAttribute
  {
    /// <summary>
    /// Verifica se o CPF é válido.
    /// </summary>
    /// <param name="value">Valor do CPF.</param>
    /// <returns>True se for válido, caso contrário false.</returns>
    public override bool IsValid(object? value)
    {
      return CpfUtils.IsCpf(value?.ToString() ?? string.Empty);
    }
  }
}