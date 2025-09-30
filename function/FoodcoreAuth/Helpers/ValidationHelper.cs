using System.ComponentModel.DataAnnotations;

namespace Foodcore.Auth.Helpers.Validation
{
  /// <summary>
  /// Classe auxiliar para validação de objetos usando Data Annotations.
  /// </summary>
  public static class ValidationHelper
  {
    /// <summary>
    /// Valida o objeto fornecido com base nas anotações de dados.
    /// </summary>
    /// <typeparam name="T">Tipo do objeto a ser validado.</typeparam>
    /// <param name="obj">Objeto a ser validado.</param>
    /// <exception cref="ValidationException"></exception>
    public static void Validate<T>(T obj)
    {
      if (obj == null)
      {
        return;
      }

      var context = new ValidationContext(obj, serviceProvider: null, items: null);
      var results = new List<ValidationResult>();

      if (!Validator.TryValidateObject(obj, context, results, true))
      {
        var errors = results.Select(r => r.ErrorMessage).ToArray();
        throw new ValidationException(errors.First());
      }
    }
  }
}