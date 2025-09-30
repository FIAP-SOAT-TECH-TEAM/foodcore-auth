namespace Foodcore.Auth.Helpers.Validation
{
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  /// Validador de Email que permite nulo ou vazio.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
  public class NullableEmailAddressAttribute : ValidationAttribute
  {
    /// <summary>
    /// Verifica se o email é válido, considerando nulo ou vazio como válido.
    /// </summary>
    /// <param name="value">Valor do email.</param>
    /// <returns>True se for válido ou vazio/nulo, caso contrário false.</returns>
    public override bool IsValid(object? value)
    {
      if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        return true;

      var emailAttribute = new EmailAddressAttribute();
      return emailAttribute.IsValid(value);
    }
  }
}
