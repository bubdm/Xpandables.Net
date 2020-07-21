using System.ComponentModel.DataAnnotations;

using Xpandables.Net.ValidatorRules;

namespace Xpandables.Samples.Desktop.Helpers
{
    public static class Validators
    {
        public static ValidationResult Validate<TArgument>(TArgument argument)
            where TArgument : class => new ModelValidator<TArgument>().IsValid(argument);
    }

    public sealed class ModelValidator<TArgument> : IValidatorRule<TArgument>
        where TArgument : class
    {
        public ValidationResult IsValid(TArgument argument)
        {
            try
            {
                Validator.ValidateObject(argument, new ValidationContext(argument, null, null), true);
                return default;
            }
            catch (ValidationException exception)
            {
                return exception.ValidationResult;
            }
        }
    }
}
