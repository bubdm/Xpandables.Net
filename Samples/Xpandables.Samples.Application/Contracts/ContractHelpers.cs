using System;
using System.ComponentModel.DataAnnotations;

namespace Xpandables.Samples.Business.Contracts
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public sealed class XpandablesValidationRuleAttribute : ValidationAttribute { }

    public static class ContractHelpers
    {
        public static ValidationException GetValidationException(
            this object _, string errorMessage, object value, params string[] memberNames)
            => new ValidationException(
                new ValidationResult(errorMessage, memberNames),
                new XpandablesValidationRuleAttribute(),
                value);
    }
}
